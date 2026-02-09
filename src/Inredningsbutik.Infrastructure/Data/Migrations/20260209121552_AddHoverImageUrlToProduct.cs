using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inredningsbutik.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHoverImageUrlToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoverImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoverImageUrl",
                table: "Products");
        }
    }
}
