using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Dto
{
    public class DeleteOrderItemModel
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public int OrderItemId { get; set; }
    }
}
