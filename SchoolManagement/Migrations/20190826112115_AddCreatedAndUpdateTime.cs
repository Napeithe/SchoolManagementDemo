using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Migrations
{
    public partial class AddCreatedAndUpdateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LimitParticipants",
                table: "GroupClass",
                newName: "ParticipantLimits");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Rooms",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "Rooms",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ParticipantGroupClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "ParticipantGroupClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "GroupLevel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "GroupLevel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "GroupClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "GroupClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "GroupClass",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AnchorGroupClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "AnchorGroupClass",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_GroupClass_RoomId",
                table: "GroupClass",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupClass_Rooms_RoomId",
                table: "GroupClass",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupClass_Rooms_RoomId",
                table: "GroupClass");

            migrationBuilder.DropIndex(
                name: "IX_GroupClass_RoomId",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ParticipantGroupClass");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "ParticipantGroupClass");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "GroupLevel");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "GroupLevel");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AnchorGroupClass");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "AnchorGroupClass");

            migrationBuilder.RenameColumn(
                name: "ParticipantLimits",
                table: "GroupClass",
                newName: "LimitParticipants");
        }
    }
}
