using E_CommerceApi.Models.Sales;
using E_CommerceApi.Repository.CategoryRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        // GET: api/Category
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAll()
        {
            List<Category> categories = await _categoryRepository.GetAll();
            if (categories is null || categories.Count == 0)
                return BadRequest("No Categories Exist!");
            return Ok(categories);
        }
        // GET: api/Category/{Id}
        [HttpGet("GetCategoryById/{Id:int}", Name = "CategoryDetailsRoute")]
        public async Task<IActionResult> GetCategoryById(int Id)
        {
            Category category = await _categoryRepository.GetById(Id);

            if (category is not null)
                return Ok(category);
            return Ok($"No Category has found with this Id: {Id}");
        }
        // POST: api/Category
        [HttpPost("AddCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostCategory(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                bool check = await _categoryRepository.AddCategory(newCategory);
                if (check == true)
                {
                    string? url = Url.Link("CategoryDetailsRoute", new { Id = newCategory.Id });
                    return Created(url, newCategory);
                }
                return Ok("This Category Exist");
            }
            return BadRequest(ModelState);
        }
        // PUT: api/Category/{Id}
        [HttpPut("UpdateCategory/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCategory([FromRoute] int Id, [FromBody] Category newCategory)
        {
            if (ModelState.IsValid)
            {
                bool check = await _categoryRepository.UpdateCategory(Id, newCategory);
                if (check == true)
                {
                    return Ok(new { Message = "Category Updated" });
                }
                return BadRequest($"The Category With Id {Id} Not Exist");
            }
            return BadRequest(ModelState);
        }
        // DELETE: api/Category/{Id}
        [HttpDelete("DeleteCategory/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int Id)
        {
            if (ModelState.IsValid)
            {
                bool check = await _categoryRepository.DeleteCategory(Id);
                if (check == true)
                {
                    return Ok("Category Deleted");
                }
                return BadRequest($"The Category With Id {Id} Not Exist");
            }
            return BadRequest(ModelState);
        }
    }
}
