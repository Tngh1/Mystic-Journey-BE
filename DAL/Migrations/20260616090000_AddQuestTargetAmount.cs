using DAL.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(MysticJourneyDbContext))]
    [Migration("20260616090000_AddQuestTargetAmount")]
    public partial class AddQuestTargetAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetAmount",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("""
                UPDATE "Quests"
                SET
                    "TargetAmount" = CASE "QuestId"
                        WHEN 1 THEN 5 WHEN 2 THEN 3 WHEN 3 THEN 10 WHEN 4 THEN 1 WHEN 5 THEN 8
                        WHEN 6 THEN 5 WHEN 7 THEN 3 WHEN 8 THEN 12 WHEN 9 THEN 1 WHEN 10 THEN 6
                        WHEN 11 THEN 4 WHEN 12 THEN 8 WHEN 13 THEN 2 WHEN 14 THEN 5 WHEN 15 THEN 1
                        WHEN 16 THEN 6 WHEN 17 THEN 4 WHEN 18 THEN 10 WHEN 19 THEN 3 WHEN 20 THEN 1
                        WHEN 21 THEN 8 WHEN 22 THEN 5 WHEN 23 THEN 12 WHEN 24 THEN 3 WHEN 25 THEN 1
                        WHEN 26 THEN 6 WHEN 27 THEN 4 WHEN 28 THEN 10 WHEN 29 THEN 2 WHEN 30 THEN 1
                        WHEN 31 THEN 8 WHEN 32 THEN 5 WHEN 33 THEN 15 WHEN 34 THEN 3 WHEN 35 THEN 1
                        WHEN 36 THEN 6 WHEN 37 THEN 4 WHEN 38 THEN 12 WHEN 39 THEN 2 WHEN 40 THEN 1
                        WHEN 41 THEN 8 WHEN 42 THEN 5 WHEN 43 THEN 15 WHEN 44 THEN 3 WHEN 45 THEN 1
                        WHEN 46 THEN 6 WHEN 47 THEN 4 WHEN 48 THEN 12 WHEN 49 THEN 2 WHEN 50 THEN 1
                        WHEN 51 THEN 5 WHEN 52 THEN 8 WHEN 53 THEN 3 WHEN 54 THEN 10 WHEN 55 THEN 1
                        WHEN 56 THEN 6 WHEN 57 THEN 4 WHEN 58 THEN 15 WHEN 59 THEN 2 WHEN 60 THEN 1
                        ELSE "TargetAmount"
                    END,
                    "RewardGold" = CASE "QuestId"
                        WHEN 1 THEN 500 WHEN 2 THEN 300 WHEN 3 THEN 800 WHEN 4 THEN 200 WHEN 5 THEN 600
                        WHEN 6 THEN 500 WHEN 7 THEN 400 WHEN 8 THEN 1000 WHEN 9 THEN 300 WHEN 10 THEN 700
                        WHEN 11 THEN 500 WHEN 12 THEN 800 WHEN 13 THEN 300 WHEN 14 THEN 600 WHEN 15 THEN 1500
                        WHEN 16 THEN 700 WHEN 17 THEN 600 WHEN 18 THEN 1000 WHEN 19 THEN 400 WHEN 20 THEN 2000
                        WHEN 21 THEN 900 WHEN 22 THEN 700 WHEN 23 THEN 1200 WHEN 24 THEN 500 WHEN 25 THEN 2500
                        WHEN 26 THEN 800 WHEN 27 THEN 700 WHEN 28 THEN 1100 WHEN 29 THEN 500 WHEN 30 THEN 3000
                        WHEN 31 THEN 1000 WHEN 32 THEN 800 WHEN 33 THEN 1500 WHEN 34 THEN 600 WHEN 35 THEN 3500
                        WHEN 36 THEN 900 WHEN 37 THEN 800 WHEN 38 THEN 1300 WHEN 39 THEN 600 WHEN 40 THEN 5000
                        WHEN 41 THEN 1100 WHEN 42 THEN 900 WHEN 43 THEN 1600 WHEN 44 THEN 700 WHEN 45 THEN 4000
                        WHEN 46 THEN 1000 WHEN 47 THEN 900 WHEN 48 THEN 1400 WHEN 49 THEN 700 WHEN 50 THEN 6000
                        WHEN 51 THEN 1000 WHEN 52 THEN 1200 WHEN 53 THEN 800 WHEN 54 THEN 1500 WHEN 55 THEN 5000
                        WHEN 56 THEN 1100 WHEN 57 THEN 1000 WHEN 58 THEN 1800 WHEN 59 THEN 800 WHEN 60 THEN 8000
                        ELSE "RewardGold"
                    END,
                    "RewardExperience" = CASE "QuestId"
                        WHEN 1 THEN 200 WHEN 2 THEN 150 WHEN 3 THEN 400 WHEN 4 THEN 100 WHEN 5 THEN 300
                        WHEN 6 THEN 250 WHEN 7 THEN 200 WHEN 8 THEN 500 WHEN 9 THEN 150 WHEN 10 THEN 350
                        WHEN 11 THEN 250 WHEN 12 THEN 400 WHEN 13 THEN 150 WHEN 14 THEN 300 WHEN 15 THEN 800
                        WHEN 16 THEN 350 WHEN 17 THEN 300 WHEN 18 THEN 500 WHEN 19 THEN 200 WHEN 20 THEN 1000
                        WHEN 21 THEN 450 WHEN 22 THEN 350 WHEN 23 THEN 600 WHEN 24 THEN 250 WHEN 25 THEN 1200
                        WHEN 26 THEN 400 WHEN 27 THEN 350 WHEN 28 THEN 550 WHEN 29 THEN 250 WHEN 30 THEN 1500
                        WHEN 31 THEN 500 WHEN 32 THEN 400 WHEN 33 THEN 750 WHEN 34 THEN 300 WHEN 35 THEN 1800
                        WHEN 36 THEN 450 WHEN 37 THEN 400 WHEN 38 THEN 650 WHEN 39 THEN 300 WHEN 40 THEN 2500
                        WHEN 41 THEN 550 WHEN 42 THEN 450 WHEN 43 THEN 800 WHEN 44 THEN 350 WHEN 45 THEN 2000
                        WHEN 46 THEN 500 WHEN 47 THEN 450 WHEN 48 THEN 700 WHEN 49 THEN 350 WHEN 50 THEN 3000
                        WHEN 51 THEN 500 WHEN 52 THEN 600 WHEN 53 THEN 400 WHEN 54 THEN 750 WHEN 55 THEN 2500
                        WHEN 56 THEN 550 WHEN 57 THEN 500 WHEN 58 THEN 900 WHEN 59 THEN 400 WHEN 60 THEN 4000
                        ELSE "RewardExperience"
                    END,
                    "RewardGems" = CASE "QuestId"
                        WHEN 1 THEN 0 WHEN 2 THEN 0 WHEN 3 THEN 5 WHEN 4 THEN 0 WHEN 5 THEN 0
                        WHEN 6 THEN 0 WHEN 7 THEN 5 WHEN 8 THEN 10 WHEN 9 THEN 0 WHEN 10 THEN 5
                        WHEN 11 THEN 0 WHEN 12 THEN 0 WHEN 13 THEN 0 WHEN 14 THEN 5 WHEN 15 THEN 20
                        WHEN 16 THEN 0 WHEN 17 THEN 0 WHEN 18 THEN 10 WHEN 19 THEN 0 WHEN 20 THEN 30
                        WHEN 21 THEN 0 WHEN 22 THEN 5 WHEN 23 THEN 10 WHEN 24 THEN 0 WHEN 25 THEN 50
                        WHEN 26 THEN 0 WHEN 27 THEN 5 WHEN 28 THEN 10 WHEN 29 THEN 0 WHEN 30 THEN 60
                        WHEN 31 THEN 0 WHEN 32 THEN 5 WHEN 33 THEN 15 WHEN 34 THEN 0 WHEN 35 THEN 80
                        WHEN 36 THEN 0 WHEN 37 THEN 5 WHEN 38 THEN 10 WHEN 39 THEN 0 WHEN 40 THEN 100
                        WHEN 41 THEN 0 WHEN 42 THEN 5 WHEN 43 THEN 15 WHEN 44 THEN 0 WHEN 45 THEN 90
                        WHEN 46 THEN 0 WHEN 47 THEN 5 WHEN 48 THEN 10 WHEN 49 THEN 0 WHEN 50 THEN 120
                        WHEN 51 THEN 0 WHEN 52 THEN 10 WHEN 53 THEN 0 WHEN 54 THEN 15 WHEN 55 THEN 100
                        WHEN 56 THEN 5 WHEN 57 THEN 0 WHEN 58 THEN 20 WHEN 59 THEN 0 WHEN 60 THEN 150
                        ELSE "RewardGems"
                    END
                WHERE "QuestId" BETWEEN 1 AND 60;
                """);

            migrationBuilder.Sql("""
                UPDATE "PlayerQuests" AS pq
                SET "TargetValue" = q."TargetAmount"
                FROM "Quests" AS q
                WHERE pq."QuestId" = q."QuestId";
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetAmount",
                table: "Quests");
        }
    }
}
