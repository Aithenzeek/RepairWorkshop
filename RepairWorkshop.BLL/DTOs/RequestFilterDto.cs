namespace RepairWorkshop.BLL.DTOs
{
    public record RequestFilterDto(
        int Page,
        int PageSize,
        string? Status,
        DateTime? DateFrom,
        DateTime? DateTo,
        string? SortBy
    );
}
