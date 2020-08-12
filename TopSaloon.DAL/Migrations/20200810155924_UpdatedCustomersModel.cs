using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class UpdatedCustomersModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompleteOrders_Customers_CustomerId",
                table: "CompleteOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CompleteOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompleteOrders_Customers_CustomerId",
                table: "CompleteOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompleteOrders_Customers_CustomerId",
                table: "CompleteOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CompleteOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CompleteOrders_Customers_CustomerId",
                table: "CompleteOrders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
