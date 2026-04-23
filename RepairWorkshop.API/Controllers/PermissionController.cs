using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController(IPermissionService service) : ControllerBase
    {
        [HttpGet("get-all-permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await service.GetAllPermissions();

            return Ok(permissions);
        }

        [HttpGet("get-one/{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await service.GetPermissionById(id);

            return Ok(permission);
        }

        [HttpPost("create-permission")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto dto)
        {
            var permission = await service.CreatePermission(dto);

            return Ok(permission);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            service.DeletePermission(id);

            return Ok();
        }

        [HttpPatch("edit-permission/{id}")]
        public async Task<IActionResult> EditPermission(int id, [FromBody] EditPermissionDto dto)
        {
            var permission = await service.EditPermission(id, dto);

            return Ok();
        }
    }
}
