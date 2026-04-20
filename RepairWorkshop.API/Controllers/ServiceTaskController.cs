using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceTaskController : ControllerBase
    {
        private readonly IServiceTaskService _service;
        public ServiceTaskController(IServiceTaskService service)
        {
            _service = service;
        }

        [HttpGet("get-all-tasks")]
        public async Task<IActionResult> GetAllServiceTasks()
        {
            var serviceTasks = await _service.GetAllServiceTasks();

            return Ok(serviceTasks);
        }

        [HttpGet("get-task/{id}")]
        public async Task<IActionResult> GetserviceTaskById(int id)
        {
            var serviceTask = await _service.GetServiceTaskById(id);

            return Ok(serviceTask);
        }

        [HttpPost("create-task")]
        public async Task<IActionResult> CreateserviceTask(int id, [FromBody] CreateServiceTaskDto dto)
        {
            var serviceTask = await _service.CreateServiceTask(id, dto);

            return Ok(serviceTask);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteserviceTask(int id)
        {
            _service.DeleteServiceTask(id);

            return Ok();
        }

        [HttpPatch("complete-task")]
        public async Task<IActionResult> CompleteserviceTask([FromBody] int id)
        {
            var serviceTask = await _service.CompleteServiceTask(id);

            return Ok(serviceTask);
        }

        [HttpPatch("cancel-task")]
        public async Task<IActionResult> CancelserviceTask([FromBody] int id)
        {
            var serviceTask = await _service.CancelServiceTask(id);

            return Ok(serviceTask);
        }

        [HttpPatch("wait-for-parts")]
        public async Task<IActionResult> WaitForserviceTaskParts([FromBody] int id)
        {
            var serviceTask = await _service.WaitForServiceTaskParts(id);

            return Ok(serviceTask);
        }

        [HttpPatch("set-on-hold-task")]
        public async Task<IActionResult> SetOnHoldserviceTask([FromBody] int id)
        {
            var serviceTask = await _service.SetOnHoldServiceTask(id);

            return Ok(serviceTask);
        }

        [HttpPatch("edit-task/{id}")]
        public async Task<IActionResult> EditserviceTask(int id, [FromBody] CreateServiceTaskDto dto)
        {
            var serviceTask = await _service.EditServiceTask(id, dto);

            return Ok(serviceTask);
        }
    }
}
