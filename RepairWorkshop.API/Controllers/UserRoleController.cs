using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _service;

        public UserRoleController(IUserRoleService service)
        {
            _service = service;
        }

        [HttpGet("get-all-user-roles")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var userRoles = await _service.GetAllUserRoles();

            return Ok(userRoles);
        }

        [HttpGet("get-one/{id}")]
        public async Task<IActionResult> GetUserRoleById(int id)
        {
            var userRole = await _service.GetUserRoleById(id);

            return Ok(userRole);
        }

        [HttpPost("create-user-role")]
        public async Task<IActionResult> CreateUserRole([FromBody] CreateUserRoleDto dto)
        {
            var userRole = await _service.CreateUserRole(dto);

            return Ok(userRole);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _service.DeleteUserRole(id);

            return Ok();
        }

        [HttpPatch("edit-user-role/{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] EditUserRoleDto dto)
        {
            var userRole = await _service.EditUserRole(id, dto);

            return Ok();
        }
    }
}
