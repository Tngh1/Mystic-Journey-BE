using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Skills",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 1,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 2,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 3,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 4,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 5,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 6,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 7,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 8,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 9,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 10,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 11,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 12,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 13,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 14,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 15,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 16,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 17,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 18,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "SkillId",
                keyValue: 19,
                column: "ImageUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Skills");
        }
    }
}
