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
        [HttpGet("get-all")]
        [HasPermission("USER_READ")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await service.GetAllUsers();

            return Ok(users);
        }

        [HttpGet("get-all-technicians")]
        [HasPermission("USER_READ")]
        public async Task<IActionResult> GetAllTechnicins()
        {
            var users = await service.GetAllTechnicians();

            return Ok(users);
        }

        [HttpGet("get-by-id/{id}")]
        [HasPermission("USER_READ")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await service.GetUserById(id);

            return Ok(user);
        }

        [HttpPost("create")]
        [HasPermission("USER_CREATE")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var user = await service.CreateUser(dto);

            return Ok(user);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("USER_DELETE")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await service.DeleteUser(id);

            return Ok();
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("USER_EDIT")]
        public async Task<IActionResult> EditUser(int id, [FromBody] EditUserDto dto)
        {
            var user = await service.EditUser(id, dto);

            return Ok();
        }
    }
}
