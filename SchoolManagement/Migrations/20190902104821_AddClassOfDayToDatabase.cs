using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SchoolManagement.Migrations
{
    public partial class AddClassOfDayToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassDayOfWeek",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: false),
                    DayOfWeek = table.Column<string>(nullable: false),
                    Hour = table.Column<TimeSpan>(nullable: false),
                    GroupClassId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassDayOfWeek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassDayOfWeek_GroupClass_GroupClassId",
                        column: x => x.GroupClassId,
                        principalTable: "GroupClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassDayOfWeek_GroupClassId",
                table: "ClassDayOfWeek",
                column: "GroupClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassDayOfWeek");
        }
    }
}
