using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IRepairItemService
    {
        public Task<ResponseRepairItemDto> CreateRepairItem(CreateRepairItemDto dto);
        public Task DeleteRepairItem(int id);
        //public Task<RepairItem> ApproveRepairItem(int id);
        public Task<ResponseRepairItemDto> StartRepairItem(int id);
        public Task<ResponseRepairItemDto> CompleteRepairItem(int id);
        public Task<ResponseRepairItemDto> CancelRepairItem(int id, CancelRepairItemDto dto);
        public Task<ResponseRepairItemDto> AllowPickUpRepairItem(int id);
        public Task<ResponseRepairItemDto> WaitForRepairItemParts(int id);
        public Task<ResponseRepairItemDto> SetOnHoldRepairItemWork(int id);
        public Task<ResponseRepairItemDto> EditRepairItem(int id, EditRepairItemDto dto);
        public Task<RepairItem?> GetRepairItemById(int id);
        public Task<List<RepairItem>> GetAllRepairItems();
        public Task<List<RepairItem>> GetAllActiveRepairItems(int workerId, bool activeOnly);
        public Task<PagedResponse<ResponseRepairItemDto>> GetPaged(int page = 1, int pageSize = 10);
        public Task<PagedResponse<ResponseRepairItemDto>> GetActivePaged(int userId, bool activeOnly, int page = 1, int pageSize = 10);
    }
}
