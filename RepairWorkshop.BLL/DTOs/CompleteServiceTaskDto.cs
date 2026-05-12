using System;
using System.Collections.Generic;
using System.Text;

namespace RepairWorkshop.BLL.DTOs
{
    public record CompleteServiceTaskDto(
        int Id,
        string? DiagnosticsResult
    );
}
