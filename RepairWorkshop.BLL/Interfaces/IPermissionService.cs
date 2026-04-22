using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IPermissionService
    {
        public Task<Permission> CreatePermission(CreatePermissionDto dto);
        public Task DeletePermission(int id);
        public Task<Permission> EditPermission(int id, EditPermissionDto dto);
        public Task<Permission> GetPermissionById(int id);
        public Task<List<Permission>> GetAllPermissions();
    }
}
