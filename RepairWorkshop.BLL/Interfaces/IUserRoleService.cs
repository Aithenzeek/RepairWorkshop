using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IUserRoleService
    {
        public Task<ResponseUserRoleDto> CreateUserRole(CreateUserRoleDto dto);
        public Task DeleteUserRole(int id);
        public Task<ResponseUserRoleDto> EditUserRole(int id, EditUserRoleDto dto);
        public Task<List<UserRole>> GetAllUserRoles();
        public Task<ResponseUserRoleDto?> GetUserRoleById(int id);
    }
}
