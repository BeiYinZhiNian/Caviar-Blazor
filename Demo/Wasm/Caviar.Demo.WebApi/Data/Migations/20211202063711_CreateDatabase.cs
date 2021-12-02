using Microsoft.EntityFrameworkCore.Migrations;

namespace Caviar.Demo.WebApi.Data.Migations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "test",
                table: "SysMenu");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "test",
                table: "SysMenu",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
