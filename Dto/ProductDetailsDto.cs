namespace E_CommerceApi.Dto
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Quntity { get; set; }
        public string Code { get; set; }
        public string ImagePath { get; set; }

        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
