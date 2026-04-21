namespace RepairWorkshop.BLL.DTOs
{
    public record CreateCustomerRequestDto(
        int CustomerId,
        int ManagerId
    );
}
