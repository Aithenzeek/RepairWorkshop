using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairWorkShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "ServiceTasks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Requests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "RepairItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ManagerId",
                table: "Requests",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_ManagerId",
                table: "Requests",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_ManagerId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ManagerId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "ServiceTasks");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "RepairItems");
        }
    }
}
