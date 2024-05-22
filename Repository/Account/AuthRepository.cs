using E_CommerceApi.Helpers;
using E_CommerceApi.Models;
using E_CommerceApi.Models.Account;
using E_CommerceApi.Models.DbContext;
using E_CommerceApi.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_CommerceApi.Repository.Account
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _Jwt;

        public AuthRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> Jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _Jwt = Jwt.Value;
        }
        // signing up new user to system
        public async Task<AuthModel> RegisterAsync(SignUpModel model)
        {
            var auth = new AuthModel();

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email Already Exist!" };
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username Already Exist!" };

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description}";
                }
                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, Roles.User.ToString());

            // Create Token
            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Message = "Signup Succeeded",
                UserName = user.UserName,
                Email = user.Email,
                TokenExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { Roles.User.ToString() },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }
        // Create Token For New User
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_Jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return JwtSecurityToken;
        }
        // Check User Login
        public async Task<AuthModel> GetTokenAsync(TokenRequistModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.Email = user.Email;
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.TokenExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            return authModel;
        }
        // Add new Role to user
        public async Task<string> AddRoleAsync(AddRoleToUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid User ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "Something went wrong!";
        }
        // Get User Name With its Id
        public async Task<string> GetNameOfUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is not null)
                return user.FirstName + " " + user.LastName;
            return string.Empty;
        }
        // Get User with Id
        public async Task<ApplicationUser> GetUserById(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is not null)
                return user;
            return null;
        }
    }
}
