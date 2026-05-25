using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Services;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.Tests.Tests
{
    public class UserServiceTests : TestBase
    {
        [Fact]
        public void CheckPhoneValidFullNumberReturnsSame()
        {
            var input = "380971234567";

            var result = UserService.CheckPhone(input);

            Assert.Equal("380971234567", result);
        }

        [Fact]
        public async Task CheckPhoneShouldCheckPhone()
        {
            var context = GetDbContext();

            var service = new UserService(context);

            var phone = "0971234567";

            var result = UserService.CheckPhone(phone);

            Assert.Equal("380971234567", result);
        }

        [Fact]
        public void CheckPhoneInvalidPhoneThrowsException()
        {
            var input = "abc123";

            Assert.Throws<BadRequestException>(() =>
                UserService.CheckPhone(input));
        }

        [Fact]
        public void CheckPhoneRemovesNonDigitsBeforeProcessing()
        {
            var input = "+380 (97) 123-45-67";

            var result = UserService.CheckPhone(input);

            Assert.Equal("380971234567", result);
        }
        [Fact]
        public async Task CreateUserShouldThrowConflictExceptionWhenUserPhoneAlreadyExists()
        {
            var context = GetDbContext();

            context.Users.Add(new User
            {
                Id = 1,
                Name = "Existing User",
                Phone = "380971234567",
                RoleId = 1
            });

            context.UserRoles.Add(new UserRole
            {
                Id = 1,
                Name = "Admin"
            });

            await context.SaveChangesAsync();

            var service = new UserService(context);

            var dto = new CreateUserDto(
                "New User",
                "380971234567",
                1
            );

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreateUser(dto)
            );
        }

        [Fact]
        public async Task CreateUserShouldThrowConflictExceptionWhenCustomerPhoneAlreadyExists()
        {
            var context = GetDbContext();

            context.Users.Add(new User
            {
                Id = 1,
                Name = "Existing User",
                Phone = "380971234567",
                RoleId = 1
            });

            context.UserRoles.Add(new UserRole
            {
                Id = 1,
                Name = "Admin"
            });

            await context.SaveChangesAsync();

            var service = new CustomerService(context);

            var dto = new CreateCustomerDto(
                "New User",
                "380971234567"
            );

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreateCustomer(dto)
            );
        }

        [Fact]
        public async Task CreateUserShouldThrowNotFoundExceptionWhenUserRoleNotFound()
        {
            var context = GetDbContext();

            var service = new UserService(context);

            var dto = new CreateUserDto(
                "New User",
                "380971234567",
                10
            );

            await Assert.ThrowsAsync<NotFoundException>(() =>
                 service.CreateUser(dto)
            );
        }
    }
}
