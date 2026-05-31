using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategoryContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryContentId",
                table: "Contents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BlockContents",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubCategoryContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CategoryContentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategoryContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategoryContents_CategoryContents_CategoryContentId",
                        column: x => x.CategoryContentId,
                        principalTable: "CategoryContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_SubCategoryContentId",
                table: "Contents",
                column: "SubCategoryContentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryContents_CategoryContentId",
                table: "SubCategoryContents",
                column: "CategoryContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_SubCategoryContents_SubCategoryContentId",
                table: "Contents",
                column: "SubCategoryContentId",
                principalTable: "SubCategoryContents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_SubCategoryContents_SubCategoryContentId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "SubCategoryContents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_SubCategoryContentId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "SubCategoryContentId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BlockContents");
        }
    }
}
