using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SchoolManagement.Migrations
{
    public partial class AddGroupLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupClass",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupLevelId",
                table: "GroupClass",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupLevel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupLevel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupClass_GroupLevelId",
                table: "GroupClass",
                column: "GroupLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupClass_GroupLevel_GroupLevelId",
                table: "GroupClass",
                column: "GroupLevelId",
                principalTable: "GroupLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupClass_GroupLevel_GroupLevelId",
                table: "GroupClass");

            migrationBuilder.DropTable(
                name: "GroupLevel");

            migrationBuilder.DropIndex(
                name: "IX_GroupClass_GroupLevelId",
                table: "GroupClass");

            migrationBuilder.DropColumn(
                name: "GroupLevelId",
                table: "GroupClass");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GroupClass",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
