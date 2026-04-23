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

            return Ok(/*repairItem*/);
        }

        [HttpDelete("delete/{id}")]
        [HasPermission("REPAIR_ITEM_DELETE")]
        public async Task<IActionResult> DeleteRepairItem(int id)
        {
            await service.DeleteRepairItem(id);

            return Ok();
        }

        [HttpPatch("complete")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> CompleteRepairItem([FromBody] int id)
        {
            var repairItem = await service.CompleteRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("cancel")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> CancelRepairItem([FromBody] int id)
        {
            var repairItem = await service.CancelRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("approve")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> ApproveRepairItem([FromBody] int id)
        {
            var repairItem = await service.ApproveRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("allow-pick-up")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> AllowPickUpRepairItem([FromBody] int id)
        {
            var repairItem = await service.AllowPickUpRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("wait-for-parts")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> WaitForRepairItemParts([FromBody] int id)
        {
            var repairItem = await service.WaitForRepairItemParts(id);

            return Ok(repairItem);
        }

        [HttpPatch("set-on-hold")]
        [HasPermission("REPAIR_ITEM_EDIT")]
        public async Task<IActionResult> SetOnHoldRepairItem([FromBody] int id)
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
    }
}
