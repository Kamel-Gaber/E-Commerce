using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Dto
{
    public class DeleteOrderModel
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int OrderId { get; set; }
    }
}
