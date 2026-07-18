using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 901,
                columns: new[] { "Description", "Name" },
                values: new object[] { "A sharp sword dropped by the SwampDemon.", "Swamp Sword" });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 902,
                columns: new[] { "Description", "Name" },
                values: new object[] { "A sturdy armor dropped by the SwampDemon.", "Swamp Armor" });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 1,
                column: "Description",
                value: "A basic slime monster.");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 2,
                column: "Description",
                value: "A dangerous swamp demon.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 901,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Thanh kiếm sắc bén rớt ra từ SwampDemon.", "Kiếm Đầm Lầy (Swamp Sword)" });

            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 902,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Chiếc áo giáp bền bỉ rớt ra từ SwampDemon.", "Giáp Đầm Lầy (Swamp Armor)" });

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 1,
                column: "Description",
                value: "Quái vật cơ bản");

            migrationBuilder.UpdateData(
                table: "Monsters",
                keyColumn: "MonsterId",
                keyValue: 2,
                column: "Description",
                value: "Quái vật đầm lầy nguy hiểm.");
        }
    }
}
