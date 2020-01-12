using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHorizon.Identity.AuthServer.Migrations.ApplicationDb
{
    public partial class DotnetUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Changed",
                table: "AutoHistory",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2048,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Changed",
                table: "AutoHistory",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
