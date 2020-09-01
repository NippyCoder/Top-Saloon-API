using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class AddedAdminAndUserImagePaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "BarberProfilePhotos");

            migrationBuilder.AddColumn<string>(
                name: "AdminPath",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPath",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminPath",
                table: "BarberProfilePhotos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserPath",
                table: "BarberProfilePhotos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminPath",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UserPath",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AdminPath",
                table: "BarberProfilePhotos");

            migrationBuilder.DropColumn(
                name: "UserPath",
                table: "BarberProfilePhotos");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "BarberProfilePhotos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
