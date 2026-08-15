using DAL.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(MysticJourneyDbContext))]
    [Migration("20260815153000_ConfigureAchievementCurrencyRewards")]
    public class ConfigureAchievementCurrencyRewards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateReward(migrationBuilder, 1, 100m, 5);
            UpdateReward(migrationBuilder, 2, 500m, 25);
            UpdateReward(migrationBuilder, 3, 250m, 15);
            UpdateReward(migrationBuilder, 4, 750m, 40);
            UpdateReward(migrationBuilder, 5, 500m, 25);
            UpdateReward(migrationBuilder, 6, 750m, 40);
            UpdateReward(migrationBuilder, 7, 1000m, 50);
            UpdateReward(migrationBuilder, 8, 1000m, 50);
            UpdateReward(migrationBuilder, 9, 1500m, 75);
            UpdateReward(migrationBuilder, 10, 2500m, 125);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int achievementId = 1; achievementId <= 10; achievementId++)
                UpdateReward(migrationBuilder, achievementId, 0m, 0);
        }

        private static void UpdateReward(
            MigrationBuilder migrationBuilder,
            int achievementId,
            decimal rewardGold,
            int rewardGem)
        {
            migrationBuilder.UpdateData(
                table: "Achievements",
                keyColumn: "AchievementId",
                keyValue: achievementId,
                columns: new[] { "RewardGold", "RewardGem" },
                values: new object[] { rewardGold, rewardGem });
        }
    }
}
