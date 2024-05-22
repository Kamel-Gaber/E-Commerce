using E_CommerceApi.Models.Sales;

namespace E_CommerceApi.Repository.CategoryRepository
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetAll();
        public Task<Category> GetById(int id);
        public Task<bool> AddCategory(Category newCategory);
        public Task<bool> UpdateCategory(int Id, Category newCategory);
        public Task<bool> DeleteCategory(int Id);
    }
}
