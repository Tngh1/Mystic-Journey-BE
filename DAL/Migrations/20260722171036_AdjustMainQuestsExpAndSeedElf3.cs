using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustMainQuestsExpAndSeedElf3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Adjust EXP for Chapter 2 main quests so completing Q9..Q13 grants 1100 EXP (reaching Level 12)
            migrationBuilder.Sql(@"
                UPDATE ""Quests"" SET ""RewardExperience"" = 100 WHERE ""QuestId"" = 9;
                UPDATE ""Quests"" SET ""RewardExperience"" = 300 WHERE ""QuestId"" = 10;
                UPDATE ""Quests"" SET ""RewardExperience"" = 200 WHERE ""QuestId"" = 11;
                UPDATE ""Quests"" SET ""RewardExperience"" = 250 WHERE ""QuestId"" = 12;
                UPDATE ""Quests"" SET ""RewardExperience"" = 250 WHERE ""QuestId"" = 13;
            ");

            // 2. Seed account elf3@mystic.test (level 12, 1100 EXP, completed Q1..Q15; Q15 is Slay the Dragon)
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    acc_id INT;
                    prof_id INT;
                    q_id INT;
                BEGIN
                    -- Find or insert account elf3@mystic.test
                    SELECT ""AccountId"" INTO acc_id FROM ""Accounts"" WHERE ""Email"" = 'elf3@mystic.test';
                    IF acc_id IS NULL THEN
                        INSERT INTO ""Accounts"" (""UserName"", ""Email"", ""HashPassword"", ""RoleId"", ""IsActive"", ""CreatedAt"")
                        VALUES ('elf3', 'elf3@mystic.test', '$2a$11$0n.e.HwH9yO3N4s0O2n3r.qV6WqJ9q4wH9yO3N4s0O2n3r.qV6WqJ', 1, true, NOW())
                        RETURNING ""AccountId"" INTO acc_id;
                    END IF;

                    -- Find or insert player profile for elf3
                    SELECT ""PlayerProfileId"" INTO prof_id FROM ""PlayerProfiles"" WHERE ""AccountId"" = acc_id;
                    IF prof_id IS NULL THEN
                        INSERT INTO ""PlayerProfiles"" (
                            ""AccountId"", ""DisplayName"", ""Class"", ""Level"", ""ExperiencePoints"",
                            ""Gold"", ""Gems"", ""CurrentEnergy"", ""MaxEnergy"",
                            ""LastEnergyUpdateTime"", ""LastActiveTime"",
                            ""LastMapName"", ""PositionX"", ""PositionY"",
                            ""AvatarUrl"", ""HasChangedName"", ""AvailableStatPoints"", ""CachedStatRolls"",
                            ""TotalDungeonClears"", ""CorruptionLevel"",
                            ""CreatedAt"", ""UpdatedAt""
                        )
                        VALUES (
                            acc_id, 'Elf 3 Mage', 'Mage', 12, 1100,
                            5000, 500, 100, 100,
                            NOW(), NOW(),
                            'AutumnPumpkin', 0, 0,
                            '', false, 11, '',
                            0, 0,
                            NOW(), NOW()
                        )
                        RETURNING ""PlayerProfileId"" INTO prof_id;
                    ELSE
                        UPDATE ""PlayerProfiles"" SET
                            ""Level"" = 12,
                            ""ExperiencePoints"" = 1100,
                            ""LastMapName"" = 'AutumnPumpkin',
                            ""UpdatedAt"" = NOW()
                        WHERE ""PlayerProfileId"" = prof_id;
                    END IF;

                    -- Clean up old player quests for elf3 profile
                    DELETE FROM ""PlayerQuests"" WHERE ""PlayerProfileId"" = prof_id;

                    -- Seed Q1 to Q15 as Claimed
                    FOR q_id IN 1..15 LOOP
                        INSERT INTO ""PlayerQuests"" (""PlayerProfileId"", ""QuestId"", ""Status"", ""Progress"", ""TargetValue"", ""AcceptedAt"", ""CompletedAt"", ""ClaimedAt"")
                        VALUES (prof_id, q_id, 'Claimed', CASE WHEN q_id = 15 THEN 10 ELSE 1 END, CASE WHEN q_id = 15 THEN 10 ELSE 1 END, NOW(), NOW(), NOW());
                    END LOOP;

                    -- Seed Q16 as Claimed
                    INSERT INTO ""PlayerQuests"" (""PlayerProfileId"", ""QuestId"", ""Status"", ""Progress"", ""TargetValue"", ""AcceptedAt"", ""CompletedAt"", ""ClaimedAt"")
                    VALUES (prof_id, 16, 'Claimed', 1, 1, NOW(), NOW(), NOW());

                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
