using E_CommerceApi.Dto;
using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Models.Sales;
using E_CommerceApi.Repository.ImageRepository;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApi.Repository.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageRepository _imageRepository;

        public ProductRepository(ApplicationDbContext context, IImageRepository imageRepository)
        {
            _context = context;
            _imageRepository = imageRepository;
        }

        public async Task<List<ProductDetailsDto>> GetAll()
        {
            List<ProductDetailsDto> products = await _context.Products
                .Include(b => b.Brand)
                .Include(c => c.Category)
                .Select(p => new ProductDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quntity = p.Quantity,
                    Code = p.Code,
                    ImagePath = p.ImagePath,
                    BrandId = p.BrandId,
                    BrandName = p.Brand.Name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name

                })
                .AsNoTracking()
                .ToListAsync();

            return products;
        }
        public async Task<Product> GetProductById(int Id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == Id);
        }
        public async Task<Product> GetProductByCode(string code)
        {
            return await _context.Products.FirstAsync(p => p.Code == code);
        }
        public async Task<ProductDetailsDto> GetByCode(string code)
        {
            var productDetails = await _context.Products
                .Where(p => p.Code == code)
                .Include(b => b.Brand)
                .Include(c => c.Category)
                .Select(p => new ProductDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quntity = p.Quantity,
                    Code = p.Code,
                    ImagePath = p.ImagePath,
                    BrandId = p.BrandId,
                    BrandName = p.Brand.Name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return productDetails;
        }
        public async Task<string> GetProductName(int Id)
        {
            Product product = await GetProductById(Id);
            if (product is not null)
                return product.Name;
            return string.Empty;
        }
        public async Task<Product> AddProduct(ProductModel newProduct)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Code == newProduct.Code);
            if (product is null)
            {
                StatusModel statusModel = await _imageRepository.UploadImage(newProduct.Image, newProduct.Code);
                if (statusModel.Flag)
                {
                    product = new Product();
                    product.Name = newProduct.Name;
                    product.Price = newProduct.Price;
                    product.Quantity = newProduct.Quantity;
                    product.Code = newProduct.Code;
                    product.BrandId = newProduct.BrandId;
                    product.CategoryId = newProduct.CategoryId;
                    product.ImagePath = _imageRepository.GetFilePath(newProduct.Code);

                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                product.Quantity += newProduct.Quantity;
                await _context.SaveChangesAsync();
            }
            return product;
        }
        public async Task<bool> UpdateProduct(int Id, Product newProduct)
        {
            Product? product = await GetProductById(Id);
            if (product is not null)
            {
                product.Name = newProduct.Name;
                product.Price = newProduct.Price;
                product.Quantity = newProduct.Quantity;
                product.Code = newProduct.Code;
                product.ImagePath = product.ImagePath;
                product.BrandId = newProduct.BrandId;
                product.CategoryId = newProduct.CategoryId;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteProduct(int Id)
        {
            Product? product = await GetProductById(Id);
            if (product is not null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<StatusModel> WithdrawProduct(int Id, int Quantity)
        {
            StatusModel statusModel = new StatusModel();
            Product product = await GetProductById(Id);
            if (product is not null)
            {
                if (product.Quantity >= Quantity)
                {
                    product.Quantity -= Quantity;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();

                    statusModel.Flag = true;
                    return statusModel;
                }
                statusModel.Flag = false;
                statusModel.Message = "Sorry, There is no enough quantity for your order";
                return statusModel;
            }
            statusModel.Flag = false;
            statusModel.Message = $"There is no product with id: {Id}";
            return statusModel;
        }
        public async Task<bool> DepositeProduct(int Id, int Quantity)
        {
            Product product = await GetProductById(Id);
            if (product is not null)
            {
                product.Quantity += Quantity;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
