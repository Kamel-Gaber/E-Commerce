using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.Dto
{
    public class ProductModel
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        [Required]
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Code { get; set; }
        public IFormFile Image { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
    }
}
