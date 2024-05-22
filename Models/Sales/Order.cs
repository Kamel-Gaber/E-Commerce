using E_CommerceApi.Models.DbContext;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_CommerceApi.Models.Sales
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        [JsonIgnore]
        public ApplicationUser? Customer { get; set; }

    }
}
