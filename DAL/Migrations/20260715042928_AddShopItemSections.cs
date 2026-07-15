using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddShopItemSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShopSection",
                table: "ShopItems",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Fixed");

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_ShopSection",
                table: "ShopItems",
                column: "ShopSection");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShopItems_ShopSection",
                table: "ShopItems");

            migrationBuilder.DropColumn(
                name: "ShopSection",
                table: "ShopItems");
        }
    }
}
