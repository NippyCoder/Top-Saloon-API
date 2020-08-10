using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class UpdatedOrdersModelAddedServiceWaitingTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalServicesWaitingTime",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalServicesWaitingTime",
                table: "Orders");
        }
    }
}
