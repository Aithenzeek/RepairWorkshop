using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController(IPermissionService service) : ControllerBase
    {
        [HttpGet("get-all")]
        [HasPermission("PERMISSION_READ")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await service.GetAllPermissions();

            return Ok(permissions);
        }

        [HttpGet("get-by-id/{id}")]
        [HasPermission("PERMISSION_READ")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await service.GetPermissionById(id);

            return Ok(permission);
        }

        [HttpPost("create")]
        [HasPermission("PERMISSION_CREATE")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto dto)
        {
            var permission = await service.CreatePermission(dto);

            return Ok(permission);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("PERMISSION_DELETE")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            await service.DeletePermission(id);

            return Ok();
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("PERMISSION_EDIT")]
        public async Task<IActionResult> EditPermission(int id, [FromBody] EditPermissionDto dto)
        {
            var permission = await service.EditPermission(id, dto);

            return Ok();
        }
    }
}
