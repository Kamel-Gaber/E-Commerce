using E_CommerceApi.Models.Sales;

namespace E_CommerceApi.Repository.BrandRepository
{
    public interface IBrandRepository
    {
        public Task<List<Brand>> GetAll();
        public Task<Brand> GetById(int id);
        public Task<bool> AddBrand(Brand newBrand);
        public Task<bool> UpdateBrand(int Id, Brand newBrand);
        public Task<bool> DeleteBrand(int Id);
    }
}
