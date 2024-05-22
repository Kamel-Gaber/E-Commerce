using E_CommerceApi.Dto;
using E_CommerceApi.Models.Sales;

namespace E_CommerceApi.Repository.ProductRepository
{
    public interface IProductRepository
    {
        public Task<List<ProductDetailsDto>> GetAll();
        public Task<Product> GetProductById(int id);
        public Task<ProductDetailsDto> GetByCode(string code);
        public Task<Product> GetProductByCode(string code);
        public Task<string> GetProductName(int Id);
        public Task<Product> AddProduct(ProductModel newProduct);
        public Task<bool> UpdateProduct(int Id, Product newProduct);
        public Task<bool> DeleteProduct(int Id);
        Task<StatusModel> WithdrawProduct(int Id, int Quantity);
        Task<bool> DepositeProduct(int Id, int Quantity);
    }
}
