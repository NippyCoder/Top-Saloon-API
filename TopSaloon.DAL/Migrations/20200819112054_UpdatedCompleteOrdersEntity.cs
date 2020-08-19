using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class UpdatedCompleteOrdersEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BarberName",
                table: "CompleteOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "CompleteOrders",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TotalTimeSpent",
                table: "CompleteOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarberName",
                table: "CompleteOrders");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CompleteOrders");

            migrationBuilder.DropColumn(
                name: "TotalTimeSpent",
                table: "CompleteOrders");
        }
    }
}
