using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IUserRoleService
    {
        public Task<UserRole> CreateUserRole(CreateUserRoleDto dto);
        public Task DeleteUserRole(int id);
        public Task<UserRole> EditUserRole(int id, EditUserRoleDto dto);
        public Task<List<UserRole>> GetAllUserRoles();
        public Task<UserRole?> GetUserRoleById(int id);
    }
}
