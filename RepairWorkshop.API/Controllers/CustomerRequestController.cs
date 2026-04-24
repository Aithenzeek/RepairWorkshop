using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.API.Authorization;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerRequestController(
        ICustomerRequestService service
    ) : ControllerBase
    {
        [HttpGet("get-all")]
        [HasPermission("REQUEST_READ")]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await service.GetAllRequests();

            return Ok(requests);
        }

        [HttpGet("get-by-id/{id}")]
        [HasPermission("REQUEST_READ")]
        public async Task<IActionResult> GetRequest(int id)
        {
            var request = await service.GetRequestById(id);

            return Ok(request);
        }

        [HttpPost("create")]
        [HasPermission("REQUEST_CREATE")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateCustomerRequestDto dto)
        {
            var request = await service.CreateRequest(dto);

            return Ok(request);
        }

        [HttpDelete("delete{id}")]
        [HasPermission("REQUEST_DELETE")]
        public async Task<IActionResult> DeleteRequest([FromRoute] int id)
        {
            await service.DeleteRequest(id);

            return Ok();
        }

        [HttpPatch("complete")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> CompleteRequest([FromBody] int id)
        {
            var request = await service.CompleteRequest(id);

            return Ok(request);
        }

        [HttpPatch("cancel")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> CancellRequest([FromBody] int id)
        {
            var request = await service.CancelRequest(id);

            return Ok(request);
        }

        [HttpPatch("approve")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> ApproveRequest([FromBody] int id)
        {
            var request = await service.ApproveRequest(id);

            return Ok(request);
        }

        [HttpPatch("allow")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> AllowPickUp([FromBody] int id)
        {
            var request = await service.AllowPickUp(id);

            return Ok(request);
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> EditRequest(int id, [FromBody] EditCustomerRequestDto dto)
        {
            var request = await service.EditRequest(id, dto);

            return Ok(request);
        }

        [HttpPatch("start/{id}")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> StartRequest([FromBody] int id)
        {
            var request = await service.StartRequest(id);

            return Ok(request);
        }
    }
}
