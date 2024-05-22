using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_CommerceApi.Models.Sales
{
    public class Product
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        [Required]
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Code { get; set; }
        public string? ImagePath { get; set; }

        [ForeignKey("Brand")]
        public int? BrandId { get; set; }
        [JsonIgnore]
        public virtual Brand? Brand { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category? Category { get; set; }

        [JsonIgnore]
        public virtual List<OrderItems>? OrderItems { get; set; }
    }
}
