using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController(ICustomerService service) : ControllerBase
    {
        [HttpGet("get-all-customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await service.GetAllCustomers();

            return Ok(customers);
        }

        [HttpGet("get-one/{phone}")]
        public async Task<IActionResult> GetCustomerByPhone(string phone)
        {
            var customer = await service.GetCustomerByPhone(phone);

            return Ok(customer);
        }

        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var customer = await service.CreateCustomer(dto);

            return Ok(customer);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            service.DeleteCustomer(id);

            return Ok();
        }

        [HttpPatch("edit-customer/{id}")]
        public async Task<IActionResult> EditCustomer(int id, [FromBody] EditCustomerDto dto)
        {
            var customer = await service.EditCustomer(id, dto);

            return Ok();
        }
    }
}
