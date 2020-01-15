using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Migrations
{
    public partial class AddNewFieldsForScheduleIngroupClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationTimeInMinutes",
                table: "GroupClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfClasses",
                table: "GroupClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartClasses",
                table: "GroupClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationTimeInMinutes",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "NumberOfClasses",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "StartClasses",
                table: "GroupClass");
        }
    }
}
