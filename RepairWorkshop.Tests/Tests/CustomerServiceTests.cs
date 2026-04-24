using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Services;

namespace RepairWorkshop.Tests.Tests
{
    public class CustomerServiceTests : TestBase
    {
        [Fact]
        public async Task CreateCustomer_ShouldCreateNewCustomer()
        {
            // Arrange
            var context = GetDbContext();

            var service = new CustomerService(context);

            var dto = new CreateCustomerDto("Test", "0971234567");

            // Act
            var result = await service.CreateCustomer(dto);

            // Assert
            Assert.NotNull(result);
        }
    }
}
