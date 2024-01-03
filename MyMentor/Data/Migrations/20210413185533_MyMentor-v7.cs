using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyMentor.Data.Migrations
{
    public partial class MyMentorv7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time Slot",
                table: "Appointment");

            migrationBuilder.AlterColumn<string>(
                name: "Birthdate",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "Start Time",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Start Time",
                table: "Appointment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthdate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time Slot",
                table: "Appointment",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
