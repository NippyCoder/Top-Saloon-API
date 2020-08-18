using Microsoft.EntityFrameworkCore.Migrations;

namespace TopSaloon.DAL.Migrations
{
    public partial class UpdatedModelsToSupportArabic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "ServiceFeedBackQuestions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderServices");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "OrderFeedbackQuestions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Barbers");

            migrationBuilder.AddColumn<string>(
                name: "NameAR",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEN",
                table: "Services",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionAR",
                table: "ServiceFeedBackQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionEN",
                table: "ServiceFeedBackQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAR",
                table: "OrderServices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEN",
                table: "OrderServices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerName",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "QuestionAR",
                table: "OrderFeedbackQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionEN",
                table: "OrderFeedbackQuestions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAR",
                table: "Barbers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEN",
                table: "Barbers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameAR",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NameEN",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "QuestionAR",
                table: "ServiceFeedBackQuestions");

            migrationBuilder.DropColumn(
                name: "QuestionEN",
                table: "ServiceFeedBackQuestions");

            migrationBuilder.DropColumn(
                name: "NameAR",
                table: "OrderServices");

            migrationBuilder.DropColumn(
                name: "NameEN",
                table: "OrderServices");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "QuestionAR",
                table: "OrderFeedbackQuestions");

            migrationBuilder.DropColumn(
                name: "QuestionEN",
                table: "OrderFeedbackQuestions");

            migrationBuilder.DropColumn(
                name: "NameAR",
                table: "Barbers");

            migrationBuilder.DropColumn(
                name: "NameEN",
                table: "Barbers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "ServiceFeedBackQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "OrderFeedbackQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Barbers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
