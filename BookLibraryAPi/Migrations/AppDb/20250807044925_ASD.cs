using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibraryAPi.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class ASD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PdfFile",
                table: "Books",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfFileName",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfFile",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PdfFileName",
                table: "Books");
        }
    }
}
