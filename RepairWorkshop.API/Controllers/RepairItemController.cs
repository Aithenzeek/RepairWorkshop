using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.DTOs;
using RepairWorkshop.BLL.Interfaces;

namespace RepairWorkshop.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RepairItemController(IRepairItemService service) : ControllerBase
    {
        [HttpGet("get-all-items")]
        public async Task<IActionResult> GetAllRepairItems()
        {
            var repairItems = await service.GetAllRepairItems();

            return Ok(repairItems);
        }

        [HttpGet("get-one/{id}")]
        public async Task<IActionResult> GetRepairItemById(int id)
        {
            var repairItem = await service.GetRepairItemById(id);

            return Ok(repairItem);
        }

        [HttpPost("create-item")]
        public async Task<IActionResult> CreateRepairItem([FromBody] CreateRepairItemDto dto)
        {
            var repairItem = await service.CreateRepairItem(dto);

            return Ok(/*repairItem*/);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRepairItem(int id)
        {
            service.DeleteRepairItem(id);

            return Ok();
        }

        [HttpPatch("complete-item")]
        public async Task<IActionResult> CompleteRepairItem([FromBody] int id)
        {
            var repairItem = await service.CompleteRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("cancel-item")]
        public async Task<IActionResult> CancelRepairItem([FromBody] int id)
        {
            var repairItem = await service.CancelRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("approve-item")]
        public async Task<IActionResult> ApproveRepairItem([FromBody] int id)
        {
            var repairItem = await service.ApproveRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("allow-pick-up-item")]
        public async Task<IActionResult> AllowPickUpRepairItem([FromBody] int id)
        {
            var repairItem = await service.AllowPickUpRepairItem(id);

            return Ok(repairItem);
        }

        [HttpPatch("wait-for-parts")]
        public async Task<IActionResult> WaitForRepairItemParts([FromBody] int id)
        {
            var repairItem = await service.WaitForRepairItemParts(id);

            return Ok(repairItem);
        }

        [HttpPatch("set-on-hold-item")]
        public async Task<IActionResult> SetOnHoldRepairItem([FromBody] int id)
        {
            var repairItem = await service.SetOnHoldRepairItemWork(id);

            return Ok(repairItem);
        }

        [HttpPatch("edit-item/{id}")]
        public async Task<IActionResult> EditRepairItem(int id, [FromBody] EditRepairItemDto dto)
        {
            var repairItem = await service.EditRepairItem(id, dto);

            return Ok(repairItem);
        }
    }
}
