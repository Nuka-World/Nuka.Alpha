using Microsoft.EntityFrameworkCore.Migrations;

namespace Nuka.Sample.API.Migrations.Sample
{
    public partial class UpdateSampleMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FNAME",
                table: "ITEMM",
                newName: "FITEMNAME");

            migrationBuilder.AddColumn<string>(
                name: "FITEMID",
                table: "ITEMM",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FITEMID",
                table: "ITEMM");

            migrationBuilder.RenameColumn(
                name: "FITEMNAME",
                table: "ITEMM",
                newName: "FNAME");
        }
    }
}
