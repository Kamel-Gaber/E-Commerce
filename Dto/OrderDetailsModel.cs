using System.Text.Json.Serialization;

namespace E_CommerceApi.Dto
{
    public class OrderDetailsModel
    {
        public int OrderId { get; set; }
        [JsonPropertyName("Customer Name")]
        public string CustomerName { get; set; }
        [JsonPropertyName("Products")]
        public List<OrderProductsModel>? OrderProducts { get; set; } = new List<OrderProductsModel>();
        [JsonPropertyName("Total Price")]
        public double TotalPrice { get; set; }
    }
}
