using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IRolePermissionService
    {
        public Task<ResponseRolePermission> CreateRolePermission(CreateRolePermissionDto dto);
        public Task DeleteRolePermission(int userRoleId, int permissionId);
        public Task<ResponseRolePermission> EditRolePermission(int userRoleId, int permissionId, EditRolePermissionDto dto);
        public Task<RolePermission?> GetRolePermissionById(int userRoleId, int permissionId);
        public Task<List<RolePermission>> GetAllRolePermissions();
    }
}
