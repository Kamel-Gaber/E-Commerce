using E_CommerceApi.Dto;
using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Models.Sales;
using E_CommerceApi.Repository.ProductRepository;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApi.Repository.OrderItemsRepository
{
    public class OrderItemsRepository : IOrderItemsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepository;

        public OrderItemsRepository(ApplicationDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<List<OrderItems>> GetAllOrderItems()
        {
            List<OrderItems> orderItems = await _context.orderItems.ToListAsync();
            return orderItems;
        }

        public async Task<OrderItems> GetOrderItemById(int Id)
        {
            OrderItems orderItem = await _context.orderItems.FirstOrDefaultAsync(o => o.Id == Id);
            return orderItem;
        }

        public async Task<List<OrderItems>> GetOrderItemsByOrderId(int Id)
        {
            return await _context.orderItems.Where(o => o.OrderId == Id).ToListAsync();
        }

        public async Task<List<OrderItems>> AddOrderItem(AddOrderModel order, int orderId)
        {
            List<OrderItems> orderItems = new List<OrderItems>();
            StatusModel statusModel = new StatusModel();
            foreach (var item in order.ProductDetails)
            {
                Product product = await _productRepository.GetProductById(item.ProductId);
                statusModel = await _productRepository.WithdrawProduct(product.Id, item.Quantity);
                if (statusModel.Flag)
                {
                    orderItems.Add(new OrderItems
                    {
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ListPrice = item.Quantity * product.Price
                    });
                }
                else
                {
                    statusModel.Flag = false; // not enough quantity for a product ==> refuse all order
                    break;
                }
            }

            if (statusModel.Flag) // if the order completed
            {
                await _context.orderItems.AddRangeAsync(orderItems);
                await _context.SaveChangesAsync();
            }
            return orderItems;
        }

        public async Task<StatusModel> UpdateOrderItem(UpdateItemModel model)
        {
            StatusModel statusModel = new StatusModel();
            OrderItems oldOrderItem = await GetOrderItemById(model.OrderItemId);
            if (oldOrderItem is not null)
            {
                Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == oldOrderItem.OrderId);
                if (model.CustomerId == order.CustomerId)
                {
                    if (model.PrducutModel.ProductId == oldOrderItem.ProductId) // if update for the same product
                    {
                        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == oldOrderItem.ProductId);
                        if (model.PrducutModel.Quantity > oldOrderItem.Quantity) // if ask for more quantity
                        {
                            int addedQuantity = model.PrducutModel.Quantity - oldOrderItem.Quantity;
                            statusModel = await _productRepository.WithdrawProduct(oldOrderItem.ProductId, addedQuantity); // first update product 
                            if (statusModel.Flag == true) // if there enough quantity for update
                            {
                                oldOrderItem.Quantity += addedQuantity; // update order item
                                oldOrderItem.ListPrice += addedQuantity * product.Price;
                                _context.orderItems.Update(oldOrderItem);
                                await _context.SaveChangesAsync();
                                statusModel.Message = "The Order Item Updated Successfully";
                            }
                            return statusModel;
                        }
                        else // if reduce the quantity of product
                        {
                            int reducedQuantity = oldOrderItem.Quantity - model.PrducutModel.Quantity;
                            statusModel.Flag = await _productRepository.DepositeProduct(oldOrderItem.ProductId, reducedQuantity); // first update product
                            if (statusModel.Flag)
                            {
                                oldOrderItem.Quantity -= reducedQuantity; // update order item
                                _context.orderItems.Update(oldOrderItem);
                                await _context.SaveChangesAsync();
                                statusModel.Message = "The order Item Updated Successfully";
                            }
                            else
                            {
                                statusModel.Flag = false;
                                statusModel.Message = $"There is no product with id: {model.PrducutModel.ProductId}";
                            }
                            return statusModel;
                        }
                    }
                    else // if update for the another product
                    {
                        Product newProduct = await _productRepository.GetProductById(model.PrducutModel.ProductId); // first check if new product exist
                        if (newProduct is not null)
                        {
                            bool flag = await _productRepository.DepositeProduct(oldOrderItem.ProductId, oldOrderItem.Quantity); // first update old product
                            if (flag)
                            {
                                statusModel = await _productRepository.WithdrawProduct(model.PrducutModel.ProductId, model.PrducutModel.Quantity); // check quantity for new product
                                if (statusModel.Flag) // if there is enough quantity
                                {
                                    oldOrderItem.ProductId = model.PrducutModel.ProductId;
                                    oldOrderItem.Quantity = model.PrducutModel.Quantity;
                                    oldOrderItem.ListPrice = newProduct.Price * model.PrducutModel.Quantity;
                                    _context.orderItems.Update(oldOrderItem);
                                    await _context.SaveChangesAsync();

                                    statusModel.Flag = true;
                                    statusModel.Message = "Your order item updated Successfully";
                                }
                                return statusModel;
                            }
                            else
                            {
                                statusModel.Flag = false;
                                statusModel.Message = $"Something Error!!";
                                return statusModel;
                            }

                        }
                        else
                        {
                            statusModel.Flag = false;
                            statusModel.Message = $"There is no product with Id: {model.PrducutModel.ProductId}";
                            return statusModel;
                        }
                    }
                }
                else
                {
                    statusModel.Flag = false;
                    statusModel.Message = "Sorry, You don't have permission to update this order Item";
                    return statusModel;
                }
            }
            statusModel.Flag = false;
            statusModel.Message = $"There is no order item with Id: {model.OrderItemId}";
            return statusModel;
        }

        public async Task<StatusModel> DeleteOrderItem(DeleteOrderItemModel model)
        {
            StatusModel statusModel = new StatusModel();
            OrderItems? orderItems = await _context.orderItems.FirstOrDefaultAsync(o => o.Id == model.OrderItemId);
            if (orderItems is not null)
            {
                Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderItems.OrderId);
                List<OrderItems> orderItemsInOrder = await GetOrderItemsByOrderId(orderItems.OrderId);
                if (orderItemsInOrder.Count == 1)
                {
                    if (order.CustomerId == model.CustomerId)
                    {
                        await _productRepository.DepositeProduct(orderItems.ProductId, orderItems.Quantity);
                        model.OrderItemId = order.Id;
                        _context.Orders.Remove(order);
                        await _context.SaveChangesAsync();
                        statusModel.Flag = true;
                        statusModel.Message = "The order Item is deleted Successfully and order also";
                        return statusModel;
                    }
                    statusModel.Flag = false;
                    statusModel.Message = "Sorry, You don't have permission to delete this order Item";
                    return statusModel;
                }
                else
                {
                    if (order.CustomerId == model.CustomerId)
                    {
                        bool flag = await _productRepository.DepositeProduct(orderItems.ProductId, orderItems.Quantity);
                        if (flag)
                        {
                            _context.orderItems.Remove(orderItems);
                            await _context.SaveChangesAsync();

                            statusModel.Flag = true;
                            statusModel.Message = "The order Item is deleted Successfully";
                            return statusModel;
                        }
                        statusModel.Flag = false;
                        statusModel.Message = "Something Error!!";
                        return statusModel;
                    }
                    statusModel.Flag = false;
                    statusModel.Message = "Sorry, You don't have permission to delete this order Item";
                    return statusModel;
                }
            }
            statusModel.Flag = false;
            statusModel.Message = $"There is no order Item with Id: {model.OrderItemId}";
            return statusModel;
        }
    }
}
