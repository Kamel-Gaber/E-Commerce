using E_CommerceApi.Dto;
using E_CommerceApi.Models.Sales;

namespace E_CommerceApi.Repository.OrderRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<List<OrderDetailsModel>> GetAllOrdersDetails();
        Task<OrderDetailsModel> GetOrderDetailsById(int Id);
        Task<Order> GetOrderById(int Id);
        Task<List<OrderDetailsModel>> GetOrdersByCustomerId(string Id);
        Task<OrderDetailsModel> AddOrder(AddOrderModel newOrder);
        Task<StatusModel> DeleteOrder(DeleteOrderModel model);
    }
}
