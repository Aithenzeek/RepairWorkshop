using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IPermissionService
    {
        public Task<ResponsePermissionDto> CreatePermission(CreatePermissionDto dto);
        public Task DeletePermission(int id);
        public Task<ResponsePermissionDto> EditPermission(int id, EditPermissionDto dto);
        public Task<Permission?> GetPermissionById(int id);
        public Task<List<Permission>> GetAllPermissions();
    }
}
