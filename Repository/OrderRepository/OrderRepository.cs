using E_CommerceApi.Dto;
using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Models.Sales;
using E_CommerceApi.Repository.Account;
using E_CommerceApi.Repository.OrderItemsRepository;
using E_CommerceApi.Repository.ProductRepository;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApi.Repository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderItemsRepository _orderItemsRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAuthRepository _authRepository;

        public OrderRepository(ApplicationDbContext context, IOrderItemsRepository orderItemsRepository, IProductRepository productRepository, IAuthRepository authRepository)
        {
            _context = context;
            _orderItemsRepository = orderItemsRepository;
            _productRepository = productRepository;
            _authRepository = authRepository;
        }
        public async Task<List<Order>> GetAllOrders()
        {
            List<Order> orders = await _context.Orders.ToListAsync();
            return orders;
        }
        public async Task<List<OrderDetailsModel>> GetAllOrdersDetails()
        {
            List<OrderDetailsModel> orderDetailsModelsList = new List<OrderDetailsModel>();
            List<Order> orders = await GetAllOrders();
            if (orders is not null && orders.Count != 0)
            {
                foreach (var order in orders)
                {
                    OrderDetailsModel orderDetailsModel = await GetOrderDetailsById(order.Id);
                    if (orderDetailsModel.OrderProducts.Count != 0)
                        orderDetailsModelsList.Add(orderDetailsModel);
                    else
                    {
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return orderDetailsModelsList;
        }
        public async Task<Order> GetOrderById(int Id)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == Id);
            return order;
        }
        public async Task<OrderDetailsModel> GetOrderDetailsById(int Id)
        {
            OrderDetailsModel orderDetailsModel = new OrderDetailsModel();
            Order order = await GetOrderById(Id);
            if (order is not null)
            {
                List<OrderItems> orderItems = await _orderItemsRepository.GetOrderItemsByOrderId(order.Id);
                orderDetailsModel.OrderProducts = new List<OrderProductsModel>();
                if (orderItems is not null)
                {
                    orderDetailsModel.OrderId = order.Id;
                    orderDetailsModel.CustomerName = await _authRepository.GetNameOfUser(order.CustomerId);
                    foreach (var item in orderItems)
                    {
                        OrderProductsModel orderDetails = new OrderProductsModel();
                        orderDetails.ItemId = item.Id;
                        orderDetails.ProductName = await _productRepository.GetProductName(item.ProductId);
                        orderDetails.Quantity = item.Quantity;
                        orderDetails.ListPrice = item.ListPrice;
                        orderDetailsModel.OrderProducts.Add(orderDetails);
                        orderDetailsModel.TotalPrice += item.ListPrice;
                    }
                }
            }
            return orderDetailsModel; // if no orderItems with this OrderId
        }
        public async Task<List<OrderDetailsModel>> GetOrdersByCustomerId(string Id)
        {
            List<OrderDetailsModel> orderDetailsModelsList = new List<OrderDetailsModel>();
            List<Order> orders = await _context.Orders.Where(o => o.CustomerId == Id).ToListAsync();
            if (orders is not null && orders.Count != 0)
            {
                foreach (var order in orders)
                {
                    OrderDetailsModel orderDetailsModel = await GetOrderDetailsById(order.Id);
                    orderDetailsModelsList.Add(orderDetailsModel);
                }
            }
            return orderDetailsModelsList; // if no orderItems with this OrderId
        }
        // proceses the order and add it
        public async Task<OrderDetailsModel> AddOrder(AddOrderModel newOrder)
        {
            OrderDetailsModel orderDetailsModel = new OrderDetailsModel();
            Order order = new Order();
            order.CustomerId = newOrder.CustomerId;
            order.OrderDate = DateTime.Now;

            order = await AddOrderToOrder(order);
            // add order to orderItems table
            List<OrderItems> orderItems = await _orderItemsRepository.AddOrderItem(newOrder, order.Id);

            if (orderItems is not null)
            {
                orderDetailsModel = await CreateOrderDetails(order, orderItems);
                return orderDetailsModel;
            }
            else // if no enough quantity of product
            {
                // newOrder.CustomerId, order.Id
                await DeleteOrder(new DeleteOrderModel { CustomerId = newOrder.CustomerId, OrderId = order.Id });
                return orderDetailsModel;
            }
        }
        // Add order to order table
        private async Task<Order> AddOrderToOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<OrderDetailsModel> CreateOrderDetails(Order order, List<OrderItems> orderItems)
        {
            OrderDetailsModel orderDetailsModel = new OrderDetailsModel();
            // get user name
            string userName = await _authRepository.GetNameOfUser(order.CustomerId);
            orderDetailsModel.OrderId = order.Id;
            orderDetailsModel.CustomerName = userName;
            if (!string.IsNullOrEmpty(userName))
            {
                foreach (var item in orderItems)
                {
                    OrderProductsModel orderDetails = new OrderProductsModel();
                    orderDetails.ItemId = item.Id;
                    orderDetails.ProductName = await _productRepository.GetProductName(item.ProductId);
                    orderDetails.Quantity = item.Quantity;
                    orderDetails.ListPrice = item.ListPrice;
                    orderDetailsModel.OrderProducts.Add(orderDetails);
                    orderDetailsModel.TotalPrice += item.ListPrice;
                }
            }
            return orderDetailsModel;
        }
        // delete order from order table
        public async Task<StatusModel> DeleteOrder(DeleteOrderModel model)
        {
            StatusModel statusModel = new StatusModel();
            Order order = await GetOrderById(model.OrderId);
            if (order is not null)
            {
                if (model.CustomerId == order.CustomerId)
                {
                    List<OrderItems> orderItems = await _orderItemsRepository.GetOrderItemsByOrderId(order.Id);
                    if (order is not null && orderItems is not null)
                    {
                        bool flag = false;
                        foreach (var item in orderItems)
                        {
                            flag = await _productRepository.DepositeProduct(item.ProductId, item.Quantity);
                        }
                        if (flag)
                        {
                            _context.Orders.Remove(order);
                            _context.orderItems.RemoveRange(orderItems);
                            await _context.SaveChangesAsync();
                            statusModel.Flag = true;
                            statusModel.Message = "The order is deleted and all OrderItems in it Successfully";
                            return statusModel;
                        }
                        else
                        {
                            statusModel.Flag = false;
                            statusModel.Message = "Something Error!!";
                            return statusModel;
                        }
                    }
                    else if (order is not null)
                    {
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                        statusModel.Flag = true;
                        statusModel.Message = "The order is deleted Successfully";
                        return statusModel;
                    }
                }
                statusModel.Flag = false;
                statusModel.Message = "Sorry, You don't have permission to delete this order";
                return statusModel;
            }
            statusModel.Flag = false;
            statusModel.Message = $"There is no order with Id: {model.OrderId}";
            return statusModel;
        }
    }
}
