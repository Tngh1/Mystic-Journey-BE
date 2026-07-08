using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClassConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassConfigs",
                columns: table => new
                {
                    ClassConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MaxHp = table.Column<int>(type: "integer", nullable: false),
                    Atk = table.Column<int>(type: "integer", nullable: false),
                    Def = table.Column<int>(type: "integer", nullable: false),
                    MoveSpeed = table.Column<int>(type: "integer", nullable: false),
                    AttackSpeed = table.Column<int>(type: "integer", nullable: false),
                    CritRate = table.Column<int>(type: "integer", nullable: false),
                    CritDamage = table.Column<int>(type: "integer", nullable: false),
                    DamageBonus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassConfigs", x => x.ClassConfigId);
                });

            migrationBuilder.InsertData(
                table: "ClassConfigs",
                columns: new[] { "ClassConfigId", "Atk", "AttackSpeed", "ClassName", "CritDamage", "CritRate", "DamageBonus", "Def", "MaxHp", "MoveSpeed" },
                values: new object[,]
                {
                    { 1, 30, 100, "Knight", 150, 5, 0, 40, 500, 100 },
                    { 2, 40, 100, "Archer", 150, 5, 0, 20, 350, 100 },
                    { 3, 50, 100, "Mage", 150, 5, 0, 15, 300, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassConfigs");
        }
    }
}
