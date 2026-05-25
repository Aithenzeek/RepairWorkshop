using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepairWorkshop.BLL.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IUserService userService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await userService.GetUserByPhone(request.Phone);

            if (user == null)
                return NotFound("User not found");

            var permissions = user.Role.RolePermissions
                .Select(rp => rp.Permission.Code)
                .ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("phone", user.Phone),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            foreach (var p in permissions)
            {
                claims.Add(new Claim("permission", p));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_123456_SUPER_SECRET_KEY_123456"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }

    public class LoginRequest
    {
        public string? Phone { get; set; }
    }
}