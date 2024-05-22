using E_CommerceApi.Models;
using E_CommerceApi.Models.Account;
using E_CommerceApi.Repository.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        // GET: api/Auth
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] SignUpModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepository.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> GetTokenAsync(TokenRequistModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepository.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("AddRoleToUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRoleToUserAsync(AddRoleToUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authRepository.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
    }
}
