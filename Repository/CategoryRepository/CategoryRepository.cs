using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Models.Sales;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApi.Repository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAll()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category> GetById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> AddCategory(Category newCategory)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == newCategory.Name);
            if (category is null)
            {
                await _context.Categories.AddAsync(newCategory);
                await _context.SaveChangesAsync();
                return true;
            }
            return false; // This Category Is Exist
        }
        public async Task<bool> UpdateCategory(int Id, Category newCategory)
        {
            Category? category = await GetById(Id);
            if (category is not null)
            {
                category.Name = newCategory.Name;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;  // This Category Not Exist
        }
        public async Task<bool> DeleteCategory(int Id)
        {
            Category? category = await GetById(Id);
            if (category is not null)
            {
                _context.Categories.Remove(category);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;  // This Category Not Exist
        }
    }
}
