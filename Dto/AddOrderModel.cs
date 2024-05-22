using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Dto
{
    public class AddOrderModel
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public List<AddproductToOrderModel> ProductDetails { get; set; }
    }
}
