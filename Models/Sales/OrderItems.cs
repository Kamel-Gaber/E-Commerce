using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApi.Models.Sales
{
    public class OrderItems
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double ListPrice { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }


        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }


    }
}
