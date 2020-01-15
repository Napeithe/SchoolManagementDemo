using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SchoolManagement.Migrations
{
    public partial class AddGroupClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    IsSolo = table.Column<bool>(nullable: false),
                    LimitParticipants = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnchorGroupClass",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    GroupClassId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnchorGroupClass", x => new { x.UserId, x.GroupClassId });
                    table.ForeignKey(
                        name: "FK_AnchorGroupClass_GroupClass_GroupClassId",
                        column: x => x.GroupClassId,
                        principalTable: "GroupClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnchorGroupClass_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantGroupClass",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    GroupClassId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantGroupClass", x => new { x.UserId, x.GroupClassId });
                    table.ForeignKey(
                        name: "FK_ParticipantGroupClass_GroupClass_GroupClassId",
                        column: x => x.GroupClassId,
                        principalTable: "GroupClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantGroupClass_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnchorGroupClass_GroupClassId",
                table: "AnchorGroupClass",
                column: "GroupClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantGroupClass_GroupClassId",
                table: "ParticipantGroupClass",
                column: "GroupClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnchorGroupClass");

            migrationBuilder.DropTable(
                name: "ParticipantGroupClass");

            migrationBuilder.DropTable(
                name: "GroupClass");
        }
    }
}
