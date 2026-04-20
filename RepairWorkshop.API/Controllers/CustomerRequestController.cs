using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("get-all-requests")]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await service.GetAllRequests();

            return Ok(requests);
        }

        [HttpGet("get-one/{id}")]
        public async Task<IActionResult> GetRequest(int id)
        {
            var request = await service.GetRequestById(id);

            return Ok(request);
        }

        [HttpPost("create-request")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateCustomerRequestDto dto)
        {
            var request = await service.CreateRequest(dto);

            return Ok(request);
        }

        [HttpDelete("delete-request{id}")]
        public async Task<IActionResult> DeleteRequest([FromRoute] int id)
        {
            service.DeleteRequest(id);

            return Ok();
        }

        [HttpPatch("complete-request")]
        public async Task<IActionResult> CompleteRequest([FromBody] int id)
        {
            var request = await service.CompleteRequest(id);

            return Ok(request);
        }

        [HttpPatch("cancel-request")]
        public async Task<IActionResult> CancellRequest([FromBody] int id)
        {
            var request = await service.CancelRequest(id);

            return Ok(request);
        }

        [HttpPatch("approve-request")]
        public async Task<IActionResult> ApproveRequest([FromBody] int id)
        {
            var request = await service.ApproveRequest(id);

            return Ok(request);
        }

        [HttpPatch("allow-pickup")]
        public async Task<IActionResult> AllowPickUp([FromBody] int id)
        {
            var request = await service.AllowPickUp(id);

            return Ok(request);
        }

        [HttpPatch("edit-request/{id}")]
        public async Task<IActionResult> EditRequest(int id, [FromBody] EditCustomerRequestDto dto)
        {
            var request = await service.EditRequest(id, dto);

            return Ok(request);
        }

        [HttpPatch("start-request/{id}")]
        public async Task<IActionResult> StartRequest([FromBody] int id)
        {
            var request = await service.StartRequest(id);

            return Ok(request);
        }
    }
}
