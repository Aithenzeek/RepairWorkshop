using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;
using RepairWorkShop.DAL.Enums;
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

            return Ok(serviceTask);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("SERVICE_TASK_DELETE")]
        public async Task<IActionResult> DeleteServiceTask(int id)
        {
            await service.DeleteServiceTask(id);

            return Ok();
        }

        [HttpPatch("start/{id}")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> StartServiceTask([FromRoute] int id)
        {
            var technicianId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var serviceTask = await service.StartServiceTask(id, technicianId);

            return Ok(serviceTask);
        }

        [HttpPatch("complete")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> CompleteServiceTask([FromBody] CompleteServiceTaskDto dto)
        {
            var technicianId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var serviceTask = await service.CompleteServiceTask(dto, technicianId);

            return Ok(serviceTask);
        }

        [HttpPatch("cancel/{id}")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> CancelServiceTask(int id, [FromBody] CancelServiceTaskDto dto)
        {
            var technicianId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var serviceTask = await service.CancelServiceTask(id, technicianId, dto);

            return Ok(serviceTask);
        }

        [HttpPatch("wait-for-parts/{id}")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> WaitForServiceTaskParts([FromRoute] int id)
        {
            var technicianId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var serviceTask = await service.WaitForServiceTaskParts(id, technicianId);

            return Ok(serviceTask);
        }

        [HttpPatch("set-on-hold/{id}")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> SetOnHoldServiceTask([FromRoute] int id)
        {
            var technicianId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var serviceTask = await service.SetOnHoldServiceTask(id, technicianId);

            return Ok(serviceTask);
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("SERVICE_TASK_EDIT")]
        public async Task<IActionResult> EditServiceTask(int id, [FromBody] EditServiceTaskDto dto)
        {
            var serviceTask = await service.EditServiceTask(id, dto);

            return Ok(serviceTask);
        }

        [HttpGet("get-paged")]
        [HasPermission("SERVICE_TASK_READ")]
        public async Task<IActionResult> GetPaged([FromQuery] ServiceTaskFilterDto filter)
        {
            var result = await service.GetPaged(filter);

            return Ok(result);
        }

        [HttpGet("get-active-paged")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> GetActivePaged(bool activeOnly, int page = 1, int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var serviceTasks = await service.GetActivePaged(userId, activeOnly, page, pageSize);

            return Ok(serviceTasks);
        }

        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            return Ok(Enum.GetNames(typeof(ServiceTaskStatus)));
        }
    }
}
