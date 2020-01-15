using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SchoolManagement.Migrations
{
    public partial class AddPass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""ParticipantGroupClass""");
            migrationBuilder.Sql(@"
                DELETE FROM ""ParticipantPresences""");

            migrationBuilder.AddColumn<int>(
                name: "PassId",
                table: "ParticipantPresences",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassId",
                table: "ParticipantGroupClass",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassId",
                table: "GroupClass",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Passes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: false),
                    NumberOfEntry = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    Paid = table.Column<bool>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passes_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantPresences_PassId",
                table: "ParticipantPresences",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantGroupClass_PassId",
                table: "ParticipantGroupClass",
                column: "PassId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupClass_PassId",
                table: "GroupClass",
                column: "PassId");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_ParticipantId",
                table: "Passes",
                column: "ParticipantId");

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
                name: "FK_ParticipantPresences_Passes_PassId",
                table: "ParticipantPresences",
                column: "PassId",
                principalTable: "Passes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupClass_Passes_PassId",
                table: "GroupClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantGroupClass_Passes_PassId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantPresences_Passes_PassId",
                table: "ParticipantPresences");

            migrationBuilder.DropTable(
                name: "Passes");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantPresences_PassId",
                table: "ParticipantPresences");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantGroupClass_PassId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropIndex(
                name: "IX_GroupClass_PassId",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "PassId",
                table: "ParticipantPresences");

            migrationBuilder.DropColumn(
                name: "PassId",
                table: "ParticipantGroupClass");

            migrationBuilder.DropColumn(
                name: "PassId",
                table: "GroupClass");
        }
    }
}
