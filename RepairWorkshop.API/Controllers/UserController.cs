using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService service) : ControllerBase
    {
        [HttpGet("get-all-users")]
        [HasPermission("USER_READ")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await service.GetAllUsers();

            return Ok(users);
        }

        [HttpGet("get-one/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await service.GetUserById(id);

            return Ok(user);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var user = await service.CreateUser(dto);

            return Ok(user);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            service.DeleteUser(id);

            return Ok();
        }

        [HttpPatch("edit-user/{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] EditUserDto dto)
        {
            var user = await service.EditUser(id, dto);

            return Ok();
        }
    }
}
