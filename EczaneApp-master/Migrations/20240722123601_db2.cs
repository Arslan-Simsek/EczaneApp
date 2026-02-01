using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EczaneApp.Migrations
{
    /// <inheritdoc />
    public partial class db2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Satislar_Musteriler_MusteriId",
                table: "Satislar");

            migrationBuilder.DropIndex(
                name: "IX_Satislar_MusteriId",
                table: "Satislar");

            migrationBuilder.DropColumn(
                name: "MusteriId",
                table: "Satislar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MusteriId",
                table: "Satislar",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Satislar_MusteriId",
                table: "Satislar",
                column: "MusteriId");

            migrationBuilder.AddForeignKey(
                name: "FK_Satislar_Musteriler_MusteriId",
                table: "Satislar",
                column: "MusteriId",
                principalTable: "Musteriler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
