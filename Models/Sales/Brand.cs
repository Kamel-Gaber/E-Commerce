using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_CommerceApi.Models.Sales
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [JsonIgnore]
        public virtual List<Product>? Products { get; set; }

    }
}