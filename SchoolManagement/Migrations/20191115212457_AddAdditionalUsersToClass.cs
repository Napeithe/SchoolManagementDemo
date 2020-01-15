using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SchoolManagement.Migrations
{
    public partial class AddAdditionalUsersToClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"delete from ""ParticipantPresences""");
            migrationBuilder.Sql(@"delete from ""Passes""");
            migrationBuilder.Sql(@"delete from ""GroupClassMembers""");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantPresences_AspNetUsers_ParticipantId",
                table: "ParticipantPresences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantPresences",
                table: "ParticipantPresences");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "ParticipantPresences",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ParticipantPresences",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<int>(
                name: "MakeUpParticipantId",
                table: "ParticipantPresences",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantPresences",
                table: "ParticipantPresences",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantPresences_ClassTimeId",
                table: "ParticipantPresences",
                column: "ClassTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantPresences_MakeUpParticipantId",
                table: "ParticipantPresences",
                column: "MakeUpParticipantId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantPresences_ParticipantPresences_MakeUpParticipant~",
                table: "ParticipantPresences",
                column: "MakeUpParticipantId",
                principalTable: "ParticipantPresences",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantPresences_AspNetUsers_ParticipantId",
                table: "ParticipantPresences",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantPresences_ParticipantPresences_MakeUpParticipant~",
                table: "ParticipantPresences");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantPresences_AspNetUsers_ParticipantId",
                table: "ParticipantPresences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantPresences",
                table: "ParticipantPresences");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantPresences_ClassTimeId",
                table: "ParticipantPresences");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantPresences_MakeUpParticipantId",
                table: "ParticipantPresences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ParticipantPresences");

            migrationBuilder.DropColumn(
                name: "MakeUpParticipantId",
                table: "ParticipantPresences");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "ParticipantPresences",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantPresences",
                table: "ParticipantPresences",
                columns: new[] { "ClassTimeId", "ParticipantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantPresences_AspNetUsers_ParticipantId",
                table: "ParticipantPresences",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
