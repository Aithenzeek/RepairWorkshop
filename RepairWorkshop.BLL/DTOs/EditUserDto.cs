using RepairWorkShop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkshop.BLL.DTOs
{
    public record EditUserDto(
        string Name,
        string Phone,
        UserRole Role
    );
}
