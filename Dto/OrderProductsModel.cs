using System.Text.Json.Serialization;

namespace E_CommerceApi.Dto
{
    public class OrderProductsModel
    {
        public int ItemId { get; set; }
        [JsonPropertyName("Product Name")]
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        [JsonPropertyName("List Price")]
        public double ListPrice { get; set; }

    }
}
