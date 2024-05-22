using E_CommerceApi.Dto;
using E_CommerceApi.Models.Sales;

namespace E_CommerceApi.Repository.OrderItemsRepository
{
    public interface IOrderItemsRepository
    {
        Task<List<OrderItems>> GetAllOrderItems();
        Task<OrderItems> GetOrderItemById(int Id);
        Task<List<OrderItems>> GetOrderItemsByOrderId(int Id);
        Task<List<OrderItems>> AddOrderItem(AddOrderModel order, int orderId);
        Task<StatusModel> UpdateOrderItem(UpdateItemModel model);
        Task<StatusModel> DeleteOrderItem(DeleteOrderItemModel model);
    }
}
