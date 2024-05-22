using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Dto
{
    public class AddproductToOrderModel
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
