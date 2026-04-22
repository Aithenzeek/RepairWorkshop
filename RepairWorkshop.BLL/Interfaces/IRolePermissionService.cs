using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IRolePermissionService
    {
        public Task<RolePermission> CreateRolePermission(CreateRolePermissionDto dto);
        public Task DeleteRolePermission(int id);
        public Task<RolePermission> EditRolePermission(int id, EditRolePermissionDto dto);
        public Task<RolePermission> GetRolePermissionById(int id);
        public Task<List<RolePermission>> GetAllRolePermissions();
    }
}
