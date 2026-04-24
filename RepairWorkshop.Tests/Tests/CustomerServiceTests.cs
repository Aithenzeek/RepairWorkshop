using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Services;

namespace RepairWorkshop.Tests.Tests
{
    public class CustomerServiceTests : TestBase
    {
        [Fact]
        public void CheckPhone_ValidFullNumber_ReturnsSame()
        {
            // Arrange
            var input = "380971234567";

            // Act
            var result = CustomerService.CheckPhone(input);

            // Assert
            Assert.Equal("380971234567", result);
        }

        [Fact]
        public async Task CheckPhone_ShouldCheckPhone()
        {
            // Arrange
            var context = GetDbContext();

            var service = new CustomerService(context);

            var phone = "0971234567";

            // Act
            var result = CustomerService.CheckPhone(phone);

            // Assert
            //Assert.NotNull(result);
            Assert.Equal("380971234567", result);
        }

        [Fact]
        public void CheckPhone_InvalidPhone_ThrowsException()
        {
            // Arrange
            var input = "abc123";

            // Act + Assert
            Assert.Throws<BadRequestException>(() =>
                CustomerService.CheckPhone(input));
        }

        [Fact]
        public void CheckPhone_RemovesNonDigitsBeforeProcessing()
        {
            // Arrange
            var input = "+380 (97) 123-45-67";

            // Act
            var result = CustomerService.CheckPhone(input);

            // Assert
            Assert.Equal("380971234567", result);
        }
    }
}
