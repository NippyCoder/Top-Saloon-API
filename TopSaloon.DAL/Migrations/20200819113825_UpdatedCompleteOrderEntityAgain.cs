using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class UpdatedCompleteOrderEntityAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarberName",
                table: "CompleteOrders");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "CompleteOrders");

            migrationBuilder.AddColumn<string>(
                name: "BarberNameAR",
                table: "CompleteOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarberNameEN",
                table: "CompleteOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNameAR",
                table: "CompleteOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNameEN",
                table: "CompleteOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarberNameAR",
                table: "CompleteOrders");

            migrationBuilder.DropColumn(
                name: "BarberNameEN",
                table: "CompleteOrders");

            migrationBuilder.DropColumn(
                name: "CustomerNameAR",
                table: "CompleteOrders");

            migrationBuilder.DropColumn(
                name: "CustomerNameEN",
                table: "CompleteOrders");

            migrationBuilder.AddColumn<string>(
                name: "BarberName",
                table: "CompleteOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "CompleteOrders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
