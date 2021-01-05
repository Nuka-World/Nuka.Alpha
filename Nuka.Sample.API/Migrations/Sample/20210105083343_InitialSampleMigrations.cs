using Microsoft.EntityFrameworkCore.Migrations;

namespace Nuka.Sample.API.Migrations.Sample
{
    public partial class InitialSampleMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleType",
                columns: table => new
                {
                    FID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FTYPE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleType", x => x.FID);
                });

            migrationBuilder.CreateTable(
                name: "ITEMM",
                columns: table => new
                {
                    FID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FNAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FDISCIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FPRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FTYPEID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITEMM", x => x.FID);
                    table.ForeignKey(
                        name: "FK_ITEMM_SampleType_FTYPEID",
                        column: x => x.FTYPEID,
                        principalTable: "SampleType",
                        principalColumn: "FID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ITEMM_FTYPEID",
                table: "ITEMM",
                column: "FTYPEID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ITEMM");

            migrationBuilder.DropTable(
                name: "SampleType");
        }
    }
}
