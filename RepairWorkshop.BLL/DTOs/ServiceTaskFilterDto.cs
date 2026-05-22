namespace RepairWorkshop.BLL.DTOs
{
    public record ServiceTaskFilterDto(
        int Page,
        int PageSize,
        string? Status,
        DateTime? DateFrom,
        DateTime? DateTo,
        int? RepairItemId,
        string? SortBy
    );
}
