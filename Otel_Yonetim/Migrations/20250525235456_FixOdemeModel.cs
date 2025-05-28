using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otel_Yonetim.Migrations
{
    /// <inheritdoc />
    public partial class FixOdemeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Odemeler_Rezervasyonlar_RezervasyonId",
                table: "Odemeler");

            migrationBuilder.AlterColumn<string>(
                name: "OdemeTuru",
                table: "Odemeler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Odemeler_Rezervasyonlar_RezervasyonId",
                table: "Odemeler",
                column: "RezervasyonId",
                principalTable: "Rezervasyonlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Odemeler_Rezervasyonlar_RezervasyonId",
                table: "Odemeler");

            migrationBuilder.AlterColumn<string>(
                name: "OdemeTuru",
                table: "Odemeler",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Odemeler_Rezervasyonlar_RezervasyonId",
                table: "Odemeler",
                column: "RezervasyonId",
                principalTable: "Rezervasyonlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
