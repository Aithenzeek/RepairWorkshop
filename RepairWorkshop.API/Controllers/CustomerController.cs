using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;
        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet("get-all-customers")]
        public IActionResult GetCustomers()
        {
            var customers = _service.GetAllCustomers();

            return Ok(customers);
        }

        [HttpGet("get-one/{phone}")]
        public IActionResult GetCustomerByPhone(string phone)
        {
            var customer = _service.GetCustomerByPhone(phone);

            return Ok(customer);
        }

        [HttpPost("create-customer")]
        public IActionResult CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var customer = _service.CreateCustomer(dto);

            return Ok(customer);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            _service.DeleteCustomer(id);

            return Ok();
        }

        [HttpPatch("edit-customer/{number}")]
        public IActionResult EditRequest(string number, [FromBody] CreateCustomerDto dto)
        {
            var request = _service.EditCustomer(number, dto);

            return Ok(request);
        }
    }
}
