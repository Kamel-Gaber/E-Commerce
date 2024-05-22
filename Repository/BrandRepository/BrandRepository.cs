using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Models.Sales;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApi.Repository.BrandRepository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ApplicationDbContext _context;
        public BrandRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Brand>> GetAll()
        {
            return await _context.Brands.ToListAsync();
        }
        public async Task<Brand> GetById(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<bool> AddBrand(Brand newBrand)
        {
            Brand? brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == newBrand.Name);
            if (brand is null)
            {
                await _context.Brands.AddAsync(newBrand);
                await _context.SaveChangesAsync();
                return true;
            }
            return false; // This Brand Is Exist
        }
        public async Task<bool> UpdateBrand(int Id, Brand newBrand)
        {
            Brand? brand = await GetById(Id);
            if (brand is not null)
            {
                brand.Name = newBrand.Name;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;  // This Brand Not Exist
        }
        public async Task<bool> DeleteBrand(int Id)
        {
            Brand? brand = await GetById(Id);
            if (brand is not null)
            {
                _context.Brands.Remove(brand);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;  // This Brand Not Exist
        }
    }
}
