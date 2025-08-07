using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibraryAPi.Migrations
{
    /// <inheritdoc />
    public partial class Init211 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad40b0ce-8a5e-4e33-8c36-123456789000",
                column: "ConcurrencyStamp",
                value: "978c99fc-3575-4c2c-ab98-79f0604c7ad8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba12c678-df89-43cb-9876-987654321000",
                column: "ConcurrencyStamp",
                value: "40533eab-72cd-4c34-8481-925d268f97b7");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad40b0ce-8a5e-4e33-8c36-123456789000",
                column: "ConcurrencyStamp",
                value: "6f56eaa5-e431-4751-a706-2f4afeb3dbc5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba12c678-df89-43cb-9876-987654321000",
                column: "ConcurrencyStamp",
                value: "f28098a7-9eeb-4736-bdcc-3811711e7090");
        }
    }
}
