using RepairWorkshop.BLL.DTOs;
using RepairWorkShop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkshop.BLL.Interfaces
{
    public interface IServiceTaskService
    {
        public Task<ServiceTask> CreateServiceTask(int repairItemId, CreateServiceTaskDto dto);
        public Task<ServiceTask> DeleteServiceTask(int id);
        public Task<ServiceTask> CompleteServiceTask(int id);
        public Task<ServiceTask> CancelServiceTask(int id);
        public Task<ServiceTask> SetOnHoldServiceTask(int id);
        public Task<ServiceTask> WaitForServiceTaskParts(int id);
        public Task<ServiceTask> EditServiceTask(int id, EditServiceTaskDto dto);
        public Task<ServiceTask> GetServiceTaskById(int id);
        public Task<List<ServiceTask>> GetAllServiceTasks();
    }
}
