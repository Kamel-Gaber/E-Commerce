using E_CommerceApi.Repository.RolesRepository.RolesRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers.Roles
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesRepository _rolesRepository;

        public RolesController(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GettAllRoles()
        {
            var roles = await _rolesRepository.GetAllRoles();
            if (roles is not null)
                return Ok(roles);
            return BadRequest("There is no Roles!");
        }
        [HttpPost("AddRole/{Name:alpha}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole([FromRoute] string Name)
        {
            if (ModelState.IsValid)
            {
                bool flag = await _rolesRepository.AddRoleAsync(Name);
                if (flag)
                    return Ok($"Role {Name} Added Successfully");
                return BadRequest("This Roles Already Exist Before");
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("DeleteRole/{Name:alpha}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole([FromRoute] string Name)
        {
            if (ModelState.IsValid)
            {
                var result = await _rolesRepository.DeleteRole(Name);
                if (result == true)
                    return Ok($"Role {Name} Deleted Successfully");
                return BadRequest("Role Not Exist Or Role has assigned users. Handle them before deletion");
            }
            return BadRequest(ModelState);
        }
    }
}
