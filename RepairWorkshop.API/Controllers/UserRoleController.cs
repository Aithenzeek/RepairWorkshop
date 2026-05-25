using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController(
        IRolePermissionService rolePermissionService,
        IUserRoleService userRoleService
    ) : ControllerBase
    {
        [HttpGet("get-all")]
        [HasPermission("USER_ROLE_READ")]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var userRoles = await userRoleService.GetAllUserRoles();

            return Ok(userRoles);
        }

        [HttpGet("get-by-id/{id}")]
        [HasPermission("USER_ROLE_READ")]
        public async Task<IActionResult> GetUserRoleById(int id)
        {
            var userRole = await userRoleService.GetUserRoleById(id);

            return Ok(userRole);
        }

        [HttpPost("create")]
        [HasPermission("USER_ROLE_CREATE")]
        public async Task<IActionResult> CreateUserRole([FromBody] CreateUserRoleDto dto)
        {
            var userRole = await userRoleService.CreateUserRole(dto);

            return Ok(userRole);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("USER_ROLE_DELETE")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await userRoleService.DeleteUserRole(id);

            return Ok();
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("USER_ROLE_EDIT")]
        public async Task<IActionResult> EditUser(int id, [FromBody] EditUserRoleDto dto)
        {
            var userRole = await userRoleService.EditUserRole(id, dto);

            return Ok();
        }

        //role permissions

        [HttpGet("get-all-role-permissions")]
        [HasPermission("ROLE_PERMISSION_READ")]
        public async Task<IActionResult> GetAllRolePermissions()
        {
            var rolePermissions = await rolePermissionService.GetAllRolePermissions();

            return Ok(rolePermissions);
        }

        [HttpGet("get-role-permission-by-id/{userRoleId}/{permissionId}")]
        [HasPermission("ROLE_PERMISSION_READ")]
        public async Task<IActionResult> GetrolePermissionById(int userRoleId, int permissionId)
        {
            var rolePermission = await rolePermissionService.GetRolePermissionById(userRoleId, permissionId);

            return Ok(rolePermission);
        }

        [HttpPost("create-role-permission")]
        [HasPermission("ROLE_PERMISSION_CREATE")]
        public async Task<IActionResult> CreateRolePermission([FromBody] CreateRolePermissionDto dto)
        {
            var rolePermission = await rolePermissionService.CreateRolePermission(dto);

            return Ok(rolePermission);
        }

        [HttpDelete("delete-role-permission/{userRoleId}/{permissionId}")]
        [HasPermission("ROLE_PERMISSION_DELETE")]
        public async Task<IActionResult> DeleteRolePermission(int userRoleId, int permissionId)
        {
            await rolePermissionService.DeleteRolePermission(userRoleId, permissionId);

            return Ok();
        }

        [HttpPatch("edit-role-permission/{userRoleId}/{permissionId}")]
        [HasPermission("ROLE_PERMISSION_EDIT")]
        public async Task<IActionResult> EditrolePermission(int userRoleId, int permissionId, [FromBody] EditRolePermissionDto dto)
        {
            var rolePermission = await rolePermissionService.EditRolePermission(userRoleId, permissionId, dto);

            return Ok();
        }

        [HttpGet("get-paged")]
        [HasPermission("ROLE_PERMISSION_READ")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var result = await rolePermissionService.GetPaged(page, pageSize);

            return Ok(result);
        }
    }
}
