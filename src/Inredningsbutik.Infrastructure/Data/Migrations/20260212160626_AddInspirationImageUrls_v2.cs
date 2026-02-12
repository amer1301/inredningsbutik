using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inredningsbutik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInspirationImageUrls_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InspirationImageUrls",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspirationImageUrls",
                table: "Products");
        }
    }
}
