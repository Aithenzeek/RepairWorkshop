using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Exceptions;
using RepairWorkshop.BLL.Services;

namespace RepairWorkshop.Tests.Tests
{
    public class CustomerServiceTests : TestBase
    {
        [Fact]
        public void CheckPhoneValidFullNumberReturnsSame()
        {
            var input = "380971234567";

            var result = CustomerService.CheckPhone(input);

            Assert.Equal("380971234567", result);
        }

        [Fact]
        public async Task CheckPhoneShouldCheckPhone()
        {
            var context = GetDbContext();

            var service = new CustomerService(context);

            var phone = "0971234567";

            var result = CustomerService.CheckPhone(phone);

            Assert.Equal("380971234567", result);
        }

        [Fact]
        public void CheckPhoneInvalidPhoneThrowsException()
        {
            var input = "abc123";

            Assert.Throws<BadRequestException>(() =>
                CustomerService.CheckPhone(input));
        }

        [Fact]
        public void CheckPhoneRemovesNonDigitsBeforeProcessing()
        {
            var input = "+380 (97) 123-45-67";

            var result = CustomerService.CheckPhone(input);

            Assert.Equal("380971234567", result);
        }
    }
}
