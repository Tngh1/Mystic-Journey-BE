using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestRewardSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestRewardSkills",
                columns: table => new
                {
                    QuestRewardSkillId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestRewardSkills", x => x.QuestRewardSkillId);
                    table.ForeignKey(
                        name: "FK_QuestRewardSkills_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "QuestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestRewardSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardSkills_QuestId_SkillId",
                table: "QuestRewardSkills",
                columns: new[] { "QuestId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardSkills_SkillId",
                table: "QuestRewardSkills",
                column: "SkillId");

            migrationBuilder.Sql(@"INSERT INTO ""QuestRewardSkills"" (""QuestId"", ""SkillId"")
SELECT ""QuestId"", ""RewardSkillId""
FROM ""Quests""
WHERE ""RewardSkillId"" IS NOT NULL
ON CONFLICT (""QuestId"", ""SkillId"") DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestRewardSkills");
        }
    }
}
