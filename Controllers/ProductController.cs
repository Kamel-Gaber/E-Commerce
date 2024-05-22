using E_CommerceApi.Dto;
using E_CommerceApi.Models.Sales;
using E_CommerceApi.Repository.ImageRepository;
using E_CommerceApi.Repository.ProductRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageRepository _imageRepository;

        public ProductController(IProductRepository productRepository, IImageRepository imageRepository)
        {
            _productRepository = productRepository;
            _imageRepository = imageRepository;
        }
        // GET: api/GetAll
        [HttpGet("GetAllProducts")]
        public async Task<ActionResult> GetAll()
        {
            List<ProductDetailsDto> products = await _productRepository.GetAll();
            if (products == null || products.Count == 0)
                return Ok("No Products Exist!");
            return Ok(products);
        }
        // GET: api/GetProductById/{Id}
        [HttpGet("GetProductById/{Id:int}", Name = "ProductDetailsRoute")]
        public async Task<IActionResult> GetProductById([FromRoute] int Id)
        {
            Product product = await _productRepository.GetProductById(Id);
            if (product is not null)
                return Ok(product);
            return Ok($"No product has found with this Id: {Id}");
        }
        // GET: api/GetProductByCode/{Id}
        [HttpGet("GetProductByCode/{Code:alpha}")]
        public async Task<IActionResult> GetProductByCode([FromRoute] string Code)
        {
            ProductDetailsDto product = await _productRepository.GetByCode(Code);
            if (product is not null)
                return Ok(product);
            return Ok($"No product has found with this Code: {Code}");
        }
        // POST: api/PostProduct
        [HttpPost("AddProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostProduct([FromForm] ProductModel newProduct)
        {
            if (ModelState.IsValid)
            {
                Product product = await _productRepository.AddProduct(newProduct);

                string? url = Url.Link("ProductDetailsRoute", new { id = product.Id });
                return Created(url, product);
            }
            return BadRequest(ModelState);
        }
        // PUT: api/PutProduct/{Id}
        [HttpPut("UpdateProduct/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int Id, [FromBody] Product newProduct)
        {
            if (ModelState.IsValid)
            {
                bool check = await _productRepository.UpdateProduct(Id, newProduct);
                if (check == true)
                {
                    newProduct.Id = Id;
                    return Ok(new { Message = "Product Updated" });
                }
                return BadRequest($"The Product With Id {Id} Not Exist");
            }
            return BadRequest(ModelState);
        }
        // DELETE: api/
        [HttpDelete("DeleteProduct/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int Id)
        {
            if (ModelState.IsValid)
            {
                bool check = await _productRepository.DeleteProduct(Id);
                if (check == true)
                {
                    return Ok("Product Deleted");
                }
                return BadRequest($"The Product With Id {Id} Not Exist");
            }
            return BadRequest(ModelState);
        }
    }
}
