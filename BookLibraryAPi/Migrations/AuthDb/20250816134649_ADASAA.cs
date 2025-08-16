using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibraryAPi.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class ADASAA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalReadingHours",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad40b0ce-8a5e-4e33-8c36-123456789000",
                column: "ConcurrencyStamp",
                value: "6192f4be-3620-4ae8-b7b8-6514f19d5904");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba12c678-df89-43cb-9876-987654321000",
                column: "ConcurrencyStamp",
                value: "7735de59-c547-4c23-af79-d5c2690d904d");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalReadingHours",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad40b0ce-8a5e-4e33-8c36-123456789000",
                column: "ConcurrencyStamp",
                value: "138754cf-b65a-4ed4-b0dc-154c229647d4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba12c678-df89-43cb-9876-987654321000",
                column: "ConcurrencyStamp",
                value: "f0b8e6b4-5ebb-4c4b-bb83-0e09dcfb710a");
        }
    }
}
