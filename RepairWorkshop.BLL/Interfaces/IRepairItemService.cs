using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IRepairItemService
    {
        public Task<RepairItem> CreateRepairItem(int requestId);
        public void DeleteRepairItem(int id);
        public Task<RepairItem> ApproveRepairItem(int id);
        public Task<RepairItem> CompleteRepairItem(int id);
        public Task<RepairItem> CancelRepairItem(int id);
        public Task<RepairItem> AllowPickUpRepairItem(int id);
        public Task<RepairItem> WaitForRepairItemParts(int id);
        public Task<RepairItem> SetOnHoldRepairItemWork(int id);
        public Task<RepairItem> EditRepairItem(int id, EditRepairItemDto dto);
        public Task<RepairItem> GetRepairItemById(int id);
        public Task<List<RepairItem>> GetAllRepairItems();
        public Task<List<RepairItem>> GetAllActiveRepairItems(int workerId);
    }
}
