using Microsoft.EntityFrameworkCore.Migrations;

namespace MyMentor.Data.Migrations
{
    public partial class MyMentorv5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Teaches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Profile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Teaches");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Course");
        }
    }
}
