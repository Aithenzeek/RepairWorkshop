using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolePermissionController(IRolePermissionService service) : ControllerBase
    {
        //[HttpGet("get-all-role-permissions")]
        //public async Task<IActionResult> GetAllRolePermissions()
        //{
        //    var rolePermissions = await service.GetAllRolePermissions();
        //
        //    return Ok(rolePermissions);
        //}

        //[HttpGet("get-one/{id}")]
        //public async Task<IActionResult> GetrolePermissionById(int id)
        //{
            //var rolePermission = await _service.GetRolePermissionById(id);

            //return Ok(rolePermission);
        //}

        //[HttpPost("create-role-permission")]
        //public async Task<IActionResult> CreateRolePermission([FromBody] CreateRolePermissionDto dto)
        //{
        //    var rolePermission = await service.CreateRolePermission(dto);
        //
        //    return Ok(rolePermission);
        //}

        //[HttpDelete("delete/{id}")]
        //public async Task<IActionResult> DeleteRolePermission(int id)
        //{
            //_service.DeleteRolePermission(id);

            //return Ok();
        //}

        //[HttpPatch("edit-role-permission/{id}")]
        //public async Task<IActionResult> EditrolePermission(int id, [FromBody] EditRolePermissionDto dto)
        //{
            //var rolePermission = await _service.EditRolePermission(id, dto);

            //return Ok();
        //}
    }
}
