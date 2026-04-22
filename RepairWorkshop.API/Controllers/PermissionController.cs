using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _service;

        public PermissionController(IPermissionService service)
        {
            _service = service;
        }

        [HttpGet("get-all-permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _service.GetAllPermissions();

            return Ok(permissions);
        }

        [HttpGet("get-one/{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _service.GetPermissionById(id);

            return Ok(permission);
        }

        [HttpPost("create-permission")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto dto)
        {
            var permission = await _service.CreatePermission(dto);

            return Ok(permission);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            _service.DeletePermission(id);

            return Ok();
        }

        [HttpPatch("edit-permission/{id}")]
        public async Task<IActionResult> EditPermission(int id, [FromBody] EditPermissionDto dto)
        {
            var permission = await _service.EditPermission(id, dto);

            return Ok();
        }
    }
}
