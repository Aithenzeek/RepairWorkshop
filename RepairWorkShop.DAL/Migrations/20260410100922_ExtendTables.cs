using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairWorkShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ExtendTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Requests",
                newName: "StartedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "Requests",
                newName: "CreatedAt");
        }
    }
}
