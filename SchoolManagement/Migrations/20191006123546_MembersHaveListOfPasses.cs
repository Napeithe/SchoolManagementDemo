using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SchoolManagement.Migrations
{
    public partial class MembersHaveListOfPasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""Passes""");

            migrationBuilder.Sql(@"
                DELETE FROM ""ParticipantGroupClass""");

            migrationBuilder.Sql(@"
                DELETE FROM ""ParticipantPresences""");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupClass_Passes_PassId",
                table: "GroupClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantGroupClass_Passes_PassId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantGroupClass_AspNetUsers_UserId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantGroupClass",
                table: "ParticipantGroupClass");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantGroupClass_PassId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropIndex(
                name: "IX_GroupClass_PassId",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "PassId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropColumn(
                name: "PassId",
                table: "GroupClass");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Passes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantGroupClassId",
                table: "Passes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Used",
                table: "Passes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ParticipantGroupClass",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ParticipantGroupClass",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantGroupClass",
                table: "ParticipantGroupClass",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_ParticipantGroupClassId",
                table: "Passes",
                column: "ParticipantGroupClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantGroupClass_UserId",
                table: "ParticipantGroupClass",
                column: "UserId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantGroupClass_AspNetUsers_UserId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropForeignKey(
                name: "FK_Passes_ParticipantGroupClass_ParticipantGroupClassId",
                table: "Passes");

            migrationBuilder.DropIndex(
                name: "IX_Passes_ParticipantGroupClassId",
                table: "Passes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantGroupClass",
                table: "ParticipantGroupClass");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantGroupClass_UserId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Passes");

            migrationBuilder.DropColumn(
                name: "ParticipantGroupClassId",
                table: "Passes");

            migrationBuilder.DropColumn(
                name: "Used",
                table: "Passes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ParticipantGroupClass");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ParticipantGroupClass",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassId",
                table: "ParticipantGroupClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassId",
                table: "GroupClass",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantGroupClass",
                table: "ParticipantGroupClass",
                columns: new[] { "UserId", "GroupClassId" });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantGroupClass_PassId",
                table: "ParticipantGroupClass",
                column: "PassId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupClass_PassId",
                table: "GroupClass",
                column: "PassId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupClass_Passes_PassId",
                table: "GroupClass",
                column: "PassId",
                principalTable: "Passes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantGroupClass_Passes_PassId",
                table: "ParticipantGroupClass",
                column: "PassId",
                principalTable: "Passes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantGroupClass_AspNetUsers_UserId",
                table: "ParticipantGroupClass",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
