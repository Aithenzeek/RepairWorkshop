using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController(ICustomerService service) : ControllerBase
    {
        [HttpGet("get-all")]
        [HasPermission("CUSTOMER_READ")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await service.GetAllCustomers();

            return Ok(customers);
        }

        [HttpGet("search-by-phone/{phone}")]
        [HasPermission("CUSTOMER_READ")]
        public async Task<IActionResult> GetCustomerByPhone(string phone)
        {
            var customer = await service.GetCustomerByPhone(phone);

            return Ok(customer);
        }

        [HttpPost("create")]
        [HasPermission("CUSTOMER_CREATE")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var customer = await service.CreateCustomer(dto);

            return Ok(customer);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("CUSTOMER_DELETE")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await service.DeleteCustomer(id);

            return Ok();
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("CUSTOMER_EDIT")]
        public async Task<IActionResult> EditCustomer(int id, [FromBody] EditCustomerDto dto)
        {
            var customer = await service.EditCustomer(id, dto);

            return Ok(customer);
        }

        [HttpGet("get-paged")]
        [HasPermission("CUSTOMER_READ")]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10)
        {
            var result = await service.GetPaged(page, pageSize);
            return Ok(result);
        }
    }
}
