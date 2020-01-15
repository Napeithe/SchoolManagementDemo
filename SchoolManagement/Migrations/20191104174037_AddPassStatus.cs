using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolManagement.Migrations
{
    public partial class AddPassStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Passes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"UPDATE public.""Passes""
                                SET ""Status"" = 'Active'
                                WHERE ""IsActive"" = true");

            migrationBuilder.Sql(@"UPDATE public.""Passes""
                                SET ""Status"" = 'NotActive'
                                WHERE ""IsActive"" = false");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Passes");
        }
    }
}
