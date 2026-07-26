using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UniquePlayerQuestPerPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Dọn các dòng trùng trước khi tạo unique index, nếu không CREATE INDEX sẽ fail.
            // Giữ lại dòng có tiến độ xa nhất (Claimed > Completed > InProgress > NotStarted),
            // rồi tới dòng mới nhất.
            migrationBuilder.Sql(@"
DELETE FROM ""PlayerQuests"" pq
USING (
    SELECT ""PlayerQuestId"",
           ROW_NUMBER() OVER (
               PARTITION BY ""PlayerProfileId"", ""QuestId""
               ORDER BY CASE ""Status""
                            WHEN 'Claimed' THEN 0
                            WHEN 'Completed' THEN 1
                            WHEN 'InProgress' THEN 2
                            ELSE 3
                        END,
                        ""AcceptedAt"" DESC,
                        ""PlayerQuestId"" DESC
           ) AS rn
    FROM ""PlayerQuests""
) dup
WHERE pq.""PlayerQuestId"" = dup.""PlayerQuestId"" AND dup.rn > 1;");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_PlayerProfileId_QuestId",
                table: "PlayerQuests",
                columns: new[] { "PlayerProfileId", "QuestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerQuests_PlayerProfileId_QuestId",
                table: "PlayerQuests");
        }
    }
}
