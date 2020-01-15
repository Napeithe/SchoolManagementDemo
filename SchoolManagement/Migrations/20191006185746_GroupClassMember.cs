using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Migrations
{
    public partial class GroupClassMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantGroupClass_GroupClass_GroupClassId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantGroupClass_AspNetUsers_UserId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropForeignKey(
                name: "FK_Passes_ParticipantGroupClass_ParticipantGroupClassId",
                table: "Passes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantGroupClass",
                table: "ParticipantGroupClass");

            migrationBuilder.RenameTable(
                name: "ParticipantGroupClass",
                newName: "GroupClassMembers");

            migrationBuilder.RenameIndex(
                name: "IX_ParticipantGroupClass_UserId",
                table: "GroupClassMembers",
                newName: "IX_GroupClassMembers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ParticipantGroupClass_GroupClassId",
                table: "GroupClassMembers",
                newName: "IX_GroupClassMembers_GroupClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupClassMembers",
                table: "GroupClassMembers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupClassMembers_GroupClass_GroupClassId",
                table: "GroupClassMembers",
                column: "GroupClassId",
                principalTable: "GroupClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupClassMembers_AspNetUsers_UserId",
                table: "GroupClassMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Passes_GroupClassMembers_ParticipantGroupClassId",
                table: "Passes",
                column: "ParticipantGroupClassId",
                principalTable: "GroupClassMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupClassMembers_GroupClass_GroupClassId",
                table: "GroupClassMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupClassMembers_AspNetUsers_UserId",
                table: "GroupClassMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Passes_GroupClassMembers_ParticipantGroupClassId",
                table: "Passes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupClassMembers",
                table: "GroupClassMembers");

            migrationBuilder.RenameTable(
                name: "GroupClassMembers",
                newName: "ParticipantGroupClass");

            migrationBuilder.RenameIndex(
                name: "IX_GroupClassMembers_UserId",
                table: "ParticipantGroupClass",
                newName: "IX_ParticipantGroupClass_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupClassMembers_GroupClassId",
                table: "ParticipantGroupClass",
                newName: "IX_ParticipantGroupClass_GroupClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantGroupClass",
                table: "ParticipantGroupClass",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantGroupClass_GroupClass_GroupClassId",
                table: "ParticipantGroupClass",
                column: "GroupClassId",
                principalTable: "GroupClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantGroupClass_AspNetUsers_UserId",
                table: "ParticipantGroupClass",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Passes_ParticipantGroupClass_ParticipantGroupClassId",
                table: "Passes",
                column: "ParticipantGroupClassId",
                principalTable: "ParticipantGroupClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
