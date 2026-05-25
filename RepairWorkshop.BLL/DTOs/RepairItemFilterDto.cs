namespace RepairWorkshop.BLL.DTOs
{
    public record RepairItemFilterDto(
        int Page,
        int PageSize,
        string? Status,
        DateTime? DateFrom,
        DateTime? DateTo,
        int? CustomerRequestId,
        string? SortBy
    );
}
