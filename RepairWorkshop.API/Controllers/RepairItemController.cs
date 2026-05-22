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
    public class RepairItemController(IRepairItemService service) : ControllerBase
    {
        [HttpGet("get-all")]
        [HasPermission("REPAIR_ITEM_READ")]
        public async Task<IActionResult> GetAllRepairItems()
        {
            var repairItems = await service.GetAllRepairItems();

            return Ok(repairItems);
        }

        [HttpGet("get-all-active")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> GetAllActiveItems(bool activeOnly)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var repairItems = await service.GetAllActiveRepairItems(userId, activeOnly);

            return Ok(repairItems);
        }

        [HttpGet("get-by-id/{id}")]
        [HasPermission("REPAIR_ITEM_READ")]
        public async Task<IActionResult> GetRepairItemById(int id)
        {
            var repairItem = await service.GetRepairItemById(id);

            return Ok(repairItem);
        }

        [HttpPost("create")]
        [HasPermission("REPAIR_ITEM_CREATE")]
        public async Task<IActionResult> CreateRepairItem([FromBody] CreateRepairItemDto dto)
        {
            var repairItem = await service.CreateRepairItem(dto);

            return Ok(repairItem);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("REPAIR_ITEM_DELETE")]
        public async Task<IActionResult> DeleteRepairItem(int id)
        {
            await service.DeleteRepairItem(id);

            return Ok();
        }

        [HttpPatch("start/{id}")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> StartRepairItem([FromRoute] int id)
        {
            var repairItem = await service.StartRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("complete/{id}")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> CompleteRepairItem([FromRoute] int id)
        {
            var repairItem = await service.CompleteRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("cancel/{id}")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> CancelRepairItem(int id, [FromBody] CancelRepairItemDto dto)
        {
            var repairItem = await service.CancelRepairItem(id, dto);

            return Ok(repairItem);
        }

        //[HttpPatch("approve")]
        //[HasPermission("REPAIR_ITEM_EDIT")]
        //public async Task<IActionResult> ApproveRepairItem([FromBody] int id)
        //{
        //    var repairItem = await service.ApproveRepairItem(id);

        //    return Ok(repairItem);
        //}

        [HttpPatch("allow-pick-up/{id}")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> AllowPickUpRepairItem([FromRoute] int id)
        {
            var repairItem = await service.AllowPickUpRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("wait-for-parts/{id}")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> WaitForRepairItemParts([FromRoute] int id)
        {
            var repairItem = await service.WaitForRepairItemParts(id);

            return Ok(repairItem);
        }

        [HttpPatch("set-on-hold/{id}")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> SetOnHoldRepairItem([FromRoute] int id)
        {
            var repairItem = await service.SetOnHoldRepairItemWork(id);

            return Ok(repairItem);
        }

        [HttpPatch("edit/{id}")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> EditRepairItem(int id, [FromBody] EditRepairItemDto dto)
        {
            var repairItem = await service.EditRepairItem(id, dto);

            return Ok(repairItem);
        }

        [HttpGet("get-paged")]
        public async Task<IActionResult> GetPaged([FromQuery] RepairItemFilterDto filter)
        {
            var result = await service.GetPaged(filter);

            return Ok(result);
        }

        [HttpGet("get-active-paged")]
        [Authorize(Roles = "Technician")]
        public async Task<IActionResult> GetActivePaged(bool activeOnly, int page = 1, int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var repairItems = await service.GetActivePaged(userId, activeOnly, page, pageSize);

            return Ok(repairItems);
        }

        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            var statuses = Enum.GetNames(typeof(RepairItemStatus));
            return Ok(statuses);
        }
    }
}
