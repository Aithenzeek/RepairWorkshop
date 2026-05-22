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
            var managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var request = await service.CreateRequest(dto, managerId);

            return Ok(request);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("REQUEST_DELETE")]
        public async Task<IActionResult> DeleteRequest([FromRoute] int id)
        {
            await service.DeleteRequest(id);

            return Ok();
        }

        [HttpPatch("complete/{id}")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> CompleteRequest([FromRoute] int id)
        {
            var request = await service.CompleteRequest(id);

            return Ok(request);
        }

        [HttpPatch("cancel/{id}")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> CancellRequest(int id, [FromBody] CancelCustomerRequestDto dto)
        {
            var request = await service.CancelRequest(id, dto);

            return Ok(request);
        }

        //[HttpPatch("approve")]
        //[HasPermission("REQUEST_EDIT")]
        //public async Task<IActionResult> ApproveRequest([FromBody] int id)
        //{
        //    var request = await service.ApproveRequest(id);

        //    return Ok(request);
        //}

        [HttpPatch("allow/{id}")]
        [HasPermission("REQUEST_EDIT")]
        public async Task<IActionResult> AllowPickUp([FromRoute] int id)
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
        public async Task<IActionResult> StartRequest([FromRoute] int id)
        {
            var request = await service.StartRequest(id);

            return Ok(request);
        }

        [HttpGet("get-paged")]
        [HasPermission("REQUEST_READ")]
        public async Task<IActionResult> GetPaged([FromQuery] RequestFilterDto filter)
        {
            var result = await service.GetPaged(filter);
            return Ok(result);
        }

        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            return Ok(Enum.GetNames(typeof(RequestStatus)));
        }
    }
}
