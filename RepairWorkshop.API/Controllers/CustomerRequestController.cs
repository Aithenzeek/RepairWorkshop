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
        public IActionResult GetRequests()
        {
            var requests = service.GetAllRequests();

            return Ok(requests);
        }

        [HttpGet("get-one/{id}")]
        public IActionResult GetRequest(int id)
        {
            var request = service.GetRequestById(id);

            return Ok(request);
        }

        [HttpPost("create-request")]
        public IActionResult CreateRequest([FromBody] int id)
        {
            var request = service.CreateRequest(id);

            return Ok(request);
        }

        [HttpDelete("delete-request{id}")]
        public IActionResult DeleteRequest([FromRoute] int id)
        {
            service.DeleteRequest(id);

            return Ok();
        }

        [HttpPatch("complete-request")]
        public IActionResult CompleteRequest([FromBody] int id)
        {
            var request = service.CompleteRequest(id);

            return Ok(request);
        }

        [HttpPatch("cancel-request")]
        public IActionResult CancellRequest([FromBody] int id)
        {
            var request = service.CancelRequest(id);

            return Ok(request);
        }

        [HttpPatch("approve-request")]
        public IActionResult ApproveRequest([FromBody] int id)
        {
            var request = service.ApproveRequest(id);

            return Ok(request);
        }

        [HttpPatch("allow-pickup")]
        public IActionResult AllowPickUp([FromBody] int id)
        {
            var request = service.AllowPickUp(id);

            return Ok(request);
        }

        [HttpPatch("edit-request/{id}")]
        public IActionResult EditRequest(int id, [FromBody] CreateCustomerRequestDto dto)
        {
            var request = service.EditRequest(id, dto);

            return Ok(request);
        }
    }
}
