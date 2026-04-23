using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IRolePermissionService
    {
        public Task<RolePermission> CreateRolePermission(CreateRolePermissionDto dto);
        public Task DeleteRolePermission(int userRoleId, int permissionId);
        public Task<RolePermission> EditRolePermission(int userRoleId, int permissionId, EditRolePermissionDto dto);
        public Task<RolePermission?> GetRolePermissionById(int userRoleId, int permissionId);
        public Task<List<RolePermission>> GetAllRolePermissions();
    }
}
