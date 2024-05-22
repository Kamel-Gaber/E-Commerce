using E_CommerceApi.Dto;
using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Repository.Account;
using E_CommerceApi.Repository.OrderItemsRepository;
using E_CommerceApi.Repository.OrderRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemsRepository _orderItemsRepository;
        private readonly IAuthRepository _authRepository;

        public OrdersController(IOrderRepository orderRepository, IOrderItemsRepository orderItemsRepository, IAuthRepository authRepository)
        {
            _orderRepository = orderRepository;
            _orderItemsRepository = orderItemsRepository;
            _authRepository = authRepository;
        }
        [HttpGet("GetAllOrders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            return Ok(await _orderRepository.GetAllOrders());
        }

        [HttpGet("GetAllOrdersDetails")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrdersDetails()
        {
            List<OrderDetailsModel> orderDetailsModel = await _orderRepository.GetAllOrdersDetails();
            if (orderDetailsModel is not null && orderDetailsModel.Count != 0)
                return Ok(orderDetailsModel);
            return BadRequest("There Is No Orders");
        }

        [HttpPost("GetOrderDetailsById/{Id:int}")]
        public async Task<IActionResult> GetOrderById([FromRoute] int Id)
        {
            if (ModelState.IsValid)
            {
                OrderDetailsModel orderDetailsModel = await _orderRepository.GetOrderDetailsById(Id);
                if (orderDetailsModel is not null && orderDetailsModel.OrderId != 0)
                    return Ok(orderDetailsModel);
                return BadRequest($"There is no order with id: {Id}");
            }
            return BadRequest(ModelState);
        }

        [HttpPost("GetOrdersByCustomerId")]
        public async Task<IActionResult> GetOrdersByCustomerId([FromBody] string Id)
        {
            if (ModelState.IsValid)
            {
                List<OrderDetailsModel> orderDetailsModel = await _orderRepository.GetOrdersByCustomerId(Id);
                if (orderDetailsModel is not null && orderDetailsModel.Count != 0)
                    return Ok(orderDetailsModel);
                return BadRequest($"There is no order with id: {Id}");
            }
            return BadRequest(ModelState);
        }

        [HttpPost("MakeOrder")]
        public async Task<IActionResult> MakeOrder(AddOrderModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _authRepository.GetUserById(model.CustomerId);
                if (user is not null)
                {
                    OrderDetailsModel orderDetailsModel = await _orderRepository.AddOrder(model);
                    if (orderDetailsModel is not null && orderDetailsModel.OrderProducts.Count != 0)
                        return Ok(orderDetailsModel);
                    return BadRequest("There is not enough quantity of a product!!");
                }
                return BadRequest("Customer Id Not Found");

            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(DeleteOrderModel model)
        {
            if (ModelState.IsValid)
            {
                StatusModel statusModel = await _orderRepository.DeleteOrder(model);
                if (statusModel.Flag)
                    return Ok(statusModel.Message);
                return BadRequest(statusModel.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("UpdateOrderItem")]
        public async Task<IActionResult> UpdateOrerItem(UpdateItemModel model)
        {
            if (ModelState.IsValid)
            {
                StatusModel statusModel = await _orderItemsRepository.UpdateOrderItem(model);
                if (statusModel.Flag)
                    return Ok(statusModel.Message);
                return BadRequest(statusModel.Message);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteOrderItem")]
        public async Task<IActionResult> DeleteOrderItem(DeleteOrderItemModel model)
        {
            if (ModelState.IsValid)
            {

                StatusModel statusModel = await _orderItemsRepository.DeleteOrderItem(model);
                if (statusModel.Flag)
                    return Ok(statusModel.Message);
                return BadRequest(statusModel.Message);
            }
            return BadRequest(ModelState);
        }
    }
}
