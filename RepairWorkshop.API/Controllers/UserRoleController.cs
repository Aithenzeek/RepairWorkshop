using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController(
        IRolePermissionService rolePermissionService,
        IUserRoleService userRoleService
    )  : ControllerBase
    {
        [HttpGet("get-all-user-roles")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var userRoles = await userRoleService.GetAllUserRoles();

            return Ok(userRoles);
        }

        [HttpGet("get-user-role/{id}")]
        public async Task<IActionResult> GetUserRoleById(int id)
        {
            var userRole = await userRoleService.GetUserRoleById(id);

            return Ok(userRole);
        }

        [HttpPost("create-user-role")]
        public async Task<IActionResult> CreateUserRole([FromBody] CreateUserRoleDto dto)
        {
            var userRole = await userRoleService.CreateUserRole(dto);

            return Ok(userRole);
        }

        [HttpDelete("delete-user-role/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            userRoleService.DeleteUserRole(id);

            return Ok();
        }

        [HttpPatch("edit-user-role/{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] EditUserRoleDto dto)
        {
            var userRole = await userRoleService.EditUserRole(id, dto);

            return Ok();
        }

        // role permissions

        [HttpGet("get-all-role-permissions")]
        public async Task<IActionResult> GetAllRolePermissions()
        {
            var rolePermissions = await rolePermissionService.GetAllRolePermissions();

            return Ok(rolePermissions);
        }

        [HttpGet("get-one")]
        public async Task<IActionResult> GetrolePermissionById(int userRoleId, int permissionId)
        {
        var rolePermission = await rolePermissionService.GetRolePermissionById(userRoleId, permissionId);

        return Ok(rolePermission);
        }

        [HttpPost("create-role-permission")]
        public async Task<IActionResult> CreateRolePermission([FromBody] CreateRolePermissionDto dto)
        {
            var rolePermission = await rolePermissionService.CreateRolePermission(dto);

            return Ok(rolePermission);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRolePermission(int userRoleId, int permissionId)
        {
        rolePermissionService.DeleteRolePermission(userRoleId, permissionId);

        return Ok();
        }

        [HttpPatch("edit-role-permission/{id}")]
        public async Task<IActionResult> EditrolePermission(int userRoleId, int permissionId, [FromBody] EditRolePermissionDto dto)
        {
        var rolePermission = await rolePermissionService.EditRolePermission(userRoleId, permissionId, dto);

        return Ok();
        }
    }
}
