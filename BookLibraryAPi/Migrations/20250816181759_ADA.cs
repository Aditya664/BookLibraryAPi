using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibraryAPi.Migrations
{
    /// <inheritdoc />
    public partial class ADA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_BookId",
                table: "Favorites",
                columns: new[] { "UserId", "BookId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_BookId",
                table: "Favorites");
        }
    }
}
