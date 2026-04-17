using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServiceController(IServiceService service)
        {
            _service = service;
        }

        [HttpGet("get-all-services")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = _service.GetAllServices();

            return Ok(services);
        }

        [HttpGet("get-one/{id}")]
        public async Task<IActionResult> GetServiceByPhone(int id)
        {
            var service = _service.GetServiceById(id);

            return Ok(service);
        }

        [HttpPost("create-service")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto dto)
        {
            var service = _service.CreateService(dto);

            return Ok(service);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            _service.DeleteService(id);

            return Ok();
        }

        [HttpPatch("edit-service/{id}")]
        public async Task<IActionResult> EditService(int id, [FromBody] EditServiceDto dto)
        {
            var service = _service.EditService(id, dto);

            return Ok();
        }
    }
}
