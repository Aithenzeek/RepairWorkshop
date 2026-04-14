using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkshop.BLL.DTOs
{
    public class CreateCustomerDto
    {
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
