using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IUserService
    {
        public Task<ResponseUserDto> CreateUser(CreateUserDto dto);
        public Task DeleteUser(int id);
        public Task<ResponseUserDto> EditUser(int id, EditUserDto dto);
        public Task<List<User>> GetAllUsers();
        public Task<List<User>> GetAllTechnicians();
        public Task<User?> GetUserById(int id);
        public Task<User?> GetUserByPhone(string phone);
        public Task<User?> SearchUserByPhone(string phone);
    }
}
