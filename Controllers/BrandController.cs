using E_CommerceApi.Models.Sales;
using E_CommerceApi.Repository.BrandRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        IBrandRepository _brandRepository;
        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        // Get: api/Brand
        [HttpGet("GetAllBrands")]
        public async Task<IActionResult> GetAll()
        {
            List<Brand> brands = await _brandRepository.GetAll();
            if (brands is null || brands.Count == 0)
                return Ok("No Brands Exist!");
            return Ok(brands);
        }
        // Get: api/Brand/{Id}
        [HttpGet("GetBrandById/{Id:int}", Name = "BrandDetailsRoute")]
        public async Task<IActionResult> GetBrandById(int Id)
        {
            Brand brand = await _brandRepository.GetById(Id);

            if (brand is not null)
                return Ok(brand);
            return Ok($"No brand has found with this Id: {Id}");
        }
        // POST: api/Brand
        [HttpPost("AddBrand")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostBrand([FromBody] Brand newBrand)
        {
            if (ModelState.IsValid)
            {
                bool check = await _brandRepository.AddBrand(newBrand);
                if (check == true)
                {
                    string? url = Url.Link("BrandDetailsRoute", new { id = newBrand.Id });
                    return Created(url, newBrand);
                }
                return BadRequest("This Brand Is Exist");
            }
            return BadRequest(ModelState);
        }
        // PUT: api/Brand/{Id}
        [HttpPut("UpdateBrand/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutBrand([FromRoute] int Id, [FromBody] Brand newBrand)
        {
            if (ModelState.IsValid)
            {
                bool check = await _brandRepository.UpdateBrand(Id, newBrand);
                if (check == true)
                {
                    return Ok(new { Message = "Brnad Updated" });
                }
                return BadRequest($"The Brand With Id {Id} Not Exist");
            }
            return BadRequest(ModelState);
        }
        // DELETE: api/Brand/{Id}
        [HttpDelete("DeleteBrand/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int Id)
        {
            if (ModelState.IsValid)
            {
                bool check = await _brandRepository.DeleteBrand(Id);
                if (check == true)
                {
                    return Ok("Brand Deleted");
                }
                return BadRequest($"The Brand With Id {Id} Not Exist");
            }
            return BadRequest(ModelState);
        }
    }
}
