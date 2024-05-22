using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Dto
{
    public class UpdateItemModel
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int OrderItemId { get; set; }
        public AddproductToOrderModel PrducutModel { get; set; }
    }
}
