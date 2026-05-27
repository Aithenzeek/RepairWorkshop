using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController(IServiceService service) : ControllerBase
    {
        private readonly IServiceService _service = service;

        [HttpGet("get-all")]
        [HasPermission("SERVICE_READ")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _service.GetAllServices();

            return Ok(services);
        }

        [HttpGet("get-all-active")]
        [HasPermission("SERVICE_READ")]
        public async Task<IActionResult> GetAllActiveServices()
        {
            var services = await _service.GetAllActiveServices();

            return Ok(services);
        }

        [HttpGet("get-by-id/{id}")]
        [HasPermission("SERVICE_READ")]
        public async Task<IActionResult> GetServiceByPhone(int id)
        {
            var service = await _service.GetServiceById(id);

            return Ok(service);
        }

        [HttpPost("create")]
        [HasPermission("SERVICE_CREATE")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDto dto)
        {
            var service = await _service.CreateService(dto);

            return Ok(service);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("SERVICE_DELETE")]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _service.DeleteService(id);

            return Ok();
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("SERVICE_EDIT")]
        public async Task<IActionResult> EditService(int id, [FromBody] EditServiceDto dto)
        {
            var service = await _service.EditService(id, dto);

            return Ok();
        }

        [HttpPatch("activate/{id}")]
        [HasPermission("SERVICE_EDIT")]
        public async Task<IActionResult> ActivateService(int id)
        {
            var service = await _service.ActivateService(id);

            return Ok();
        }

        [HttpPatch("inactivate/{id}")]
        [HasPermission("SERVICE_EDIT")]
        public async Task<IActionResult> InactivateService(int id)
        {
            var service = await _service.InactivateService(id);

            return Ok();
        }
    }
}
