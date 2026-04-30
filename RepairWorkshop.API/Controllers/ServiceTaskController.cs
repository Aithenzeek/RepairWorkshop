using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;
using System.Security.Claims;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceTaskController(IServiceTaskService service) : ControllerBase
    {
        [HttpGet("get-all")]
        [HasPermission("SERVICE_TASK_READ")]
        public async Task<IActionResult> GetAllServiceTasks()
        {
            var serviceTasks = await service.GetAllServiceTasks();

            return Ok(serviceTasks);
        }

        [HttpGet("get-all-active")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> GetAllActiveServiceTasks(bool activeOnly)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var serviceTasks = await service.GetAllActiveServiceTasks(userId, activeOnly);

            return Ok(serviceTasks);
        }

        [HttpGet("get-by-id/{id}")]
        [HasPermission("SERVICE_TASK_READ")]
        public async Task<IActionResult> GetServiceTaskById(int id)
        {
            var serviceTask = await service.GetServiceTaskById(id);

            return Ok(serviceTask);
        }

        [HttpPost("create")]
        [HasPermission("SERVICE_TASK_CREATE")]
        public async Task<IActionResult> CreateServiceTask([FromBody] CreateServiceTaskDto dto)
        {
            var serviceTask = await service.CreateServiceTask(dto);

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("SERVICE_TASK_DELETE")]
        public async Task<IActionResult> DeleteServiceTask(int id)
        {
            await service.DeleteServiceTask(id);

            return Ok();
        }

        [HttpPatch("start")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> StartServiceTask([FromBody] int id)
        {
            var serviceTask = await service.StartServiceTask(id);

            return Ok();
        }

        [HttpPatch("complete")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> CompleteServiceTask([FromBody] CompleteServiceTaskDto dto)
        {
            var serviceTask = await service.CompleteServiceTask(dto);

            return Ok();
        }

        [HttpPatch("cancel")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> CancelServiceTask([FromBody] int id)
        {
            var serviceTask = await service.CancelServiceTask(id);

            return Ok();
        }

        [HttpPatch("wait-for-parts")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> WaitForServiceTaskParts([FromBody] int id)
        {
            var serviceTask = await service.WaitForServiceTaskParts(id);

            return Ok();
        }

        [HttpPatch("set-on-hold")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> SetOnHoldServiceTask([FromBody] int id)
        {
            var serviceTask = await service.SetOnHoldServiceTask(id);

            return Ok();
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> EditServiceTask(int id, [FromBody] EditServiceTaskDto dto)
        {
            var serviceTask = await service.EditServiceTask(id, dto);

            return Ok(serviceTask);
        }
    }
}
