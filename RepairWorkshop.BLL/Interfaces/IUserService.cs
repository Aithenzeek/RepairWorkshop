using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IUserService
    {
        public Task<User> CreateUser(CreateUserDto dto);
        public Task DeleteUser(int id);
        public Task<User> EditUser(int id, EditUserDto dto);
        public Task<List<User>> GetAllUsers();
        public Task<User?> GetUserById(int id);
    }
}
