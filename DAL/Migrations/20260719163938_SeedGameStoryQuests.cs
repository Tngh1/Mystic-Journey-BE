using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedGameStoryQuests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Dark Poison Zone skill if missing (needed for Q3 reward)
            migrationBuilder.Sql(@"
            INSERT INTO ""Skills"" (""Name"", ""Description"", ""Type"", ""DamageType"", ""TargetType"", ""ClassRequirement"", ""CooldownSeconds"", ""BaseDamage"", ""DamagePerLevel"", ""DamageGrowthPercent"", ""CorruptionCost"", ""UnlockLevel"", ""IsActive"")
            SELECT 'Dark Poison Zone', 'Tạo bãi độc gây sát thương diện rộng.', 'Active', 'Magical', 'Area', 'All', 90, 80.0, 10.0, 3.0, 10, 1, true
            WHERE NOT EXISTS (SELECT 1 FROM ""Skills"" WHERE ""Name"" = 'Dark Poison Zone');
            ");

            migrationBuilder.Sql(@"
-- Seed Items
INSERT INTO ""Items"" (""Name"", ""Description"", ""Type"", ""Rarity"", ""Slot"", ""BaseValue"", ""MaxStack"", ""IsActive"", ""CorruptionReduction"", ""CreatedAt"") VALUES
('Enchanted Pumpkin', 'A magical pumpkin glowing with autumn energy.', 'QuestItem', 'Common', 'None', 0, 99, true, 0, NOW()),
('Magic Flour', 'Mystical flour used for special spells.', 'QuestItem', 'Common', 'None', 0, 99, true, 0, NOW()),
('Spirit Skull', 'A skull radiating with ghostly presence.', 'QuestItem', 'Common', 'None', 0, 99, true, 0, NOW()),
('Mystic Key', 'A key that opens the castle on the deserted island.', 'QuestItem', 'Epic', 'None', 0, 1, true, 0, NOW())
ON CONFLICT DO NOTHING;

-- Seed NPCs
INSERT INTO ""NPCs"" (""Name"", ""Description"", ""Type"", ""MapName"", ""PositionX"", ""PositionY"", ""InteractionRadius"", ""IsActive"") VALUES
('Elder Rowan', 'The wise guide of the Elf Forest.', 'QuestGiver', 'ElfForest', 12.4932, 18.61223, 2.5, true),
('Lyra', 'A spirit of the forest.', 'QuestGiver', 'ElfForest', 41.94587, -27.18052, 2.5, true),
('Mysterious Figure', 'A mysterious figure in a cloak.', 'QuestGiver', 'ElfForest', 10.11194, -45.86301, 2.5, true),
('Elder Rowan (Pumpkin)', 'The wise guide, now in the pumpkin town.', 'QuestGiver', 'AutumnPumpkin', 1.873512, -92.8158, 2.5, true),
('Tristan', 'The city gate guard.', 'QuestGiver', 'AutumnPumpkin', 11.62283, -113.6158, 2.5, true),
('Arthur', 'The silver knight.', 'QuestGiver', 'AutumnPumpkin', 275.64, -206.91, 2.5, true),
('Roselyn Aurora Queen', 'Queen of the frozen lands.', 'QuestGiver', 'FrozenMountain', 160.8554, -35.6486, 2.5, true),
('Zephyr', 'The witch and disguised priest.', 'QuestGiver', 'FrozenMountain', 6.996814, -0.2094555, 2.5, true),
('Roland', 'The forbidden zone guard.', 'QuestGiver', 'FrozenMountain', 70.45686, 18.80354, 2.5, true),
('Valiant Warrior', 'A brave warrior fighting skeletons.', 'QuestGiver', 'AbandonedCastle', -10.66112, 54.92884, 2.5, true),
('Natalie', 'The ghost of a young girl.', 'QuestGiver', 'AbandonedCastle', -48.92126, -21.12006, 2.5, true),
('Elf Guard', 'The lone guard of the deserted island.', 'QuestGiver', 'AbandonedCastle', -6.237758, -13.13438, 2.5, true)
ON CONFLICT DO NOTHING;

DO $$
DECLARE
    npc_rowan1 INT; npc_lyra INT; npc_mystery INT;
    npc_rowan2 INT; npc_tristan INT; npc_arthur INT;
    npc_queen INT; npc_zephyr INT; npc_roland INT;
    npc_valiant INT; npc_natalie INT; npc_elfguard INT;
    q_id INT;
BEGIN
    SELECT ""NPCId"" INTO npc_rowan1 FROM ""NPCs"" WHERE ""Name"" = 'Elder Rowan' AND ""MapName"" = 'ElfForest' LIMIT 1;
    SELECT ""NPCId"" INTO npc_lyra FROM ""NPCs"" WHERE ""Name"" = 'Lyra' LIMIT 1;
    SELECT ""NPCId"" INTO npc_mystery FROM ""NPCs"" WHERE ""Name"" = 'Mysterious Figure' LIMIT 1;
    SELECT ""NPCId"" INTO npc_rowan2 FROM ""NPCs"" WHERE ""Name"" = 'Elder Rowan (Pumpkin)' LIMIT 1;
    SELECT ""NPCId"" INTO npc_tristan FROM ""NPCs"" WHERE ""Name"" = 'Tristan' LIMIT 1;
    SELECT ""NPCId"" INTO npc_arthur FROM ""NPCs"" WHERE ""Name"" = 'Arthur' LIMIT 1;
    SELECT ""NPCId"" INTO npc_queen FROM ""NPCs"" WHERE ""Name"" = 'Roselyn Aurora Queen' LIMIT 1;
    SELECT ""NPCId"" INTO npc_zephyr FROM ""NPCs"" WHERE ""Name"" = 'Zephyr' LIMIT 1;
    SELECT ""NPCId"" INTO npc_roland FROM ""NPCs"" WHERE ""Name"" = 'Roland' LIMIT 1;
    SELECT ""NPCId"" INTO npc_valiant FROM ""NPCs"" WHERE ""Name"" = 'Valiant Warrior' LIMIT 1;
    SELECT ""NPCId"" INTO npc_natalie FROM ""NPCs"" WHERE ""Name"" = 'Natalie' LIMIT 1;
    SELECT ""NPCId"" INTO npc_elfguard FROM ""NPCs"" WHERE ""Name"" = 'Elf Guard' LIMIT 1;

    -- MAP 1
    -- Q1
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Speak with Elder Rowan', 'Talk to Elder Rowan in the Elf Forest.', 'Main', 'NotStarted', 'ElfForest', 1, 1, 50, 100, 0, 'Talk', 'Elder Rowan', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_rowan1, q_id, 'Quest', 'Welcome to the Elf Forest, young traveler. We have much to do.', 1, true);

    -- Q2
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Gather White Flowers', 'Collect 3 White Flowers from the forest.', 'Main', 'NotStarted', 'ElfForest', 1, 3, 120, 80, 0, 'Collect', 'White Flower', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_rowan1, q_id, 'Quest', 'Please gather 3 White Flowers for me from the old willow clearing.', 1, true);

    -- Q3
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""RewardSkillId"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Deliver White Flowers', 'Deliver the gathered flowers to Elder Rowan.', 'Main', 'NotStarted', 'ElfForest', 1, 1, 50, 50, 0, (SELECT ""SkillId"" FROM ""Skills"" WHERE ""Name"" = 'Dark Poison Zone' LIMIT 1), 'Talk', 'Elder Rowan', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_rowan1, q_id, 'Reward', 'Thank you! These will be very useful.', 1, true);

    -- Q4
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Equip Your Skill', 'Equip your first combat skill.', 'Main', 'NotStarted', 'ElfForest', 1, 1, 100, 100, 0, 'EquipSkill', 'Skill Panel', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_rowan1, q_id, 'Quest', 'Before you face real danger, you must learn to use your skills. Equip your first skill.', 1, true);

    -- Q5
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Defeat Slimes', 'Kill 3 SlimeLittle monsters in the forest.', 'Main', 'NotStarted', 'ElfForest', 1, 3, 150, 150, 0, 'Defeat', 'SlimeLittle', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_rowan1, q_id, 'Quest', 'The forest is overrun with slimes. Defeat 3 of them to prove your worth.', 1, true);

    -- Q6
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 1] The Swamp Demon', 'Slay the Swamp Demon and obtain its Seal Book.', 'Main', 'NotStarted', 'ElfForest', 2, 1, 300, 500, 0, 'Defeat', 'SwampDemon', 'Deep Woods', 2, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_rowan1, q_id, 'Quest', 'A terrible Swamp Demon haunts the deep woods. Destroy it and claim the Swamp Seal Book.', 1, true);

    -- Q7
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] The Origin Tree', 'Talk to Lyra about the cursed Origin Tree and the 4 Seal Books.', 'Main', 'NotStarted', 'ElfForest', 2, 1, 100, 100, 0, 'Talk', 'Lyra', 'Origin Tree', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_lyra, q_id, 'Quest', 'The Origin Tree has been cursed. Only the 4 Seal Books can cleanse it. You must find the remaining three.', 1, true);

    -- Q8
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] The Mysterious Figure', 'Follow the cloaked figure through the portal to Autumn Pumpkin.', 'Main', 'NotStarted', 'ElfForest', 2, 1, 50, 50, 0, 'Explore', 'Portal', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_mystery, q_id, 'Quest', 'Heh... If you want the truth, follow me through this portal.', 1, true);

    -- MAP 2
    -- Q9
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] Gather Pumpkins', 'Help Elder Rowan by collecting 15 Enchanted Pumpkins.', 'Main', 'NotStarted', 'AutumnPumpkin', 3, 15, 200, 200, 0, 'Collect', 'Enchanted Pumpkin', 'Pumpkin Town', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_rowan2, q_id, 'Quest', 'We need supplies. Gather 15 Enchanted Pumpkins for me.', 1, true);

    -- Q10
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] The Ruined City', 'Enter the city, find the mysterious figure, and talk to Tristan.', 'Main', 'NotStarted', 'AutumnPumpkin', 3, 1, 100, 100, 0, 'Talk', 'Tristan', 'City Gate', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_tristan, q_id, 'None', 'Halt! The city is in ruins. A mysterious cloaked figure passed by here recently.', 1, true);

    -- Q11
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] The Silver Knight', 'Follow Tristan''s advice and find Arthur, the silver knight.', 'Main', 'NotStarted', 'AutumnPumpkin', 4, 1, 150, 150, 0, 'Talk', 'Arthur', 'Ruined City', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_arthur, q_id, 'None', 'Greetings, warrior. A dragon is massacring our people. We must stop it.', 1, true);

    -- Q12
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 2] Slay the Dragon', 'Defeat DragonBossIdle to obtain the Dragon Seal Book.', 'Main', 'NotStarted', 'AutumnPumpkin', 5, 1, 500, 1000, 0, 'Defeat', 'DragonBossIdle', 'Ruined City', 7, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_arthur, q_id, 'Quest', 'Slay the DragonBossIdle! Bring peace to this ruined city.', 1, true);

    -- Q13
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] The Frozen Threat', 'Talk to Arthur to learn about the codex in the Frozen Mountains.', 'Main', 'NotStarted', 'AutumnPumpkin', 5, 1, 100, 100, 0, 'Talk', 'Arthur', 'Ruined City', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_arthur, q_id, 'Quest', 'Thank you! Now, you must head to the Frozen Mountains. The codex is tearing that land apart.', 1, true);

    -- MAP 3
    -- Q14
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""RewardItemId"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] The Ice Slimes', 'Meet Queen Roselyn Aurora and defeat 8 Ice Slimes.', 'Main', 'NotStarted', 'FrozenMountain', 6, 8, 300, 300, 0, (SELECT ""ItemId"" FROM ""Items"" WHERE ""Name"" = 'Magic Flour' LIMIT 1), 'Defeat', 'slime_ice', 'Snow Fields', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_queen, q_id, 'Quest', 'This land is devastated by the codex. Only volunteers remain. Please, clear out 8 ice slimes.', 1, true);

    -- Q15
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] Magic Flour for the Priest', 'Deliver Magic Flour (obtained from the Queen) to the Priest (Zephyr).', 'Main', 'NotStarted', 'FrozenMountain', 6, 1, 150, 150, 0, 'Talk', 'Zephyr', 'Frozen Mountain', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_queen, q_id, 'Quest', 'Take this Magic Flour and deliver it to our Priest, Zephyr.', 1, true);

    -- Q16
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] Dragons of Snow', 'Meet Zephyr and slay 5 Ice Dragons on the mountain.', 'Main', 'NotStarted', 'FrozenMountain', 7, 5, 400, 400, 0, 'Defeat', 'Ice_Dragon', 'Frozen Mountain', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_zephyr, q_id, 'Quest', 'Ah, the flour! Let me tell you about the codex and the beasts... Go slay 5 Ice Dragons on the mountain.', 1, true);

    -- Q17
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] The Forbidden Zone', 'Head to the forbidden zone and speak with Roland to explore it.', 'Main', 'NotStarted', 'FrozenMountain', 7, 1, 150, 150, 0, 'Explore', 'Roland', 'Forbidden Zone', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_roland, q_id, 'None', 'Halt! This is the forbidden zone. But since you made it this far, help me explore it.', 1, true);

    -- Q18
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 3] Truth of the Codex', 'Discover the truth of the codex and defeat GolemBoss to get the Golem Seal Book.', 'Main', 'NotStarted', 'FrozenMountain', 8, 1, 800, 1500, 0, 'Defeat', 'GolemBoss', 'Forbidden Zone', 10, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_roland, q_id, 'Quest', 'The truth is terrifying. Defeat the giant GolemBoss to claim the Golem Seal Book!', 1, true);

    -- MAP 4
    -- Q19
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Skeleton Army', 'Defeat 12 skeletons in the valley for Valiant Warrior.', 'Main', 'NotStarted', 'AbandonedCastle', 9, 12, 500, 500, 0, 'Defeat', 'Skeleton', 'Valley', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_valiant, q_id, 'Quest', 'An ancient power is leaking, causing skeletons to multiply out of control. Defeat 12 of them!', 1, true);

    -- Q20
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] The Abandoned Village', 'Go to Tide-Knell village, meet Natalie, and dig up the skull near the old well.', 'Main', 'NotStarted', 'AbandonedCastle', 9, 1, 300, 300, 0, 'Interact', 'Skull', 'Tide-Knell', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_valiant, q_id, 'Quest', 'The animals are fleeing from the abandoned village Tide-Knell. Investigate it and find Natalie.', 1, true);
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_natalie, q_id, 'Quest', 'Please... dig up what is buried under the small tree near the old well.', 2, true);

    -- Q21
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""RewardItemId"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Rest in Peace', 'Read Natalie''s suicide letter and bury her remains under the ivy tree. Receive Mystic Key.', 'Main', 'NotStarted', 'AbandonedCastle', 10, 1, 400, 400, 0, (SELECT ""ItemId"" FROM ""Items"" WHERE ""Name"" = 'Mystic Key' LIMIT 1), 'Talk', 'Natalie', 'Tide-Knell', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_natalie, q_id, 'Reward', 'Thank you for bringing my remains home. The ancient power leak was my doing. Take this key to the island castle.', 1, true);

    -- Q22
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Deserted Island', 'Talk to Elf Guard on the deserted island and collect 5 Ancient Leaves.', 'Main', 'NotStarted', 'AbandonedCastle', 10, 5, 450, 450, 0, 'Collect', 'Ancient Leaves', 'Northern Plateau', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_elfguard, q_id, 'Quest', 'You survived the waves. Help me collect 5 Ancient Leaves from the Northern Plateau.', 1, true);

    -- Q23
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 4] The UnderKing', 'Defeat the UnderKing to claim the final UnderKing Seal Book.', 'Main', 'NotStarted', 'AbandonedCastle', 11, 1, 2000, 3000, 0, 'Defeat', 'UnderKing', 'Deserted Island', 15, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_elfguard, q_id, 'Quest', 'The UnderKing guards the final Seal Book. You must end his reign!', 1, true);

    -- Q24
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Return to Elf Forest', 'Talk to Elf Guard. He will open a portal back to the Elf Forest.', 'Main', 'NotStarted', 'AbandonedCastle', 12, 1, 100, 100, 0, 'Talk', 'Elf Guard', 'Deserted Island', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_elfguard, q_id, 'Reward', 'Farewell, hero. I will use my power to open a portal back to the Elf Forest. Save the Origin Tree!', 1, true);

    -- Q25
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Save the Origin Tree', 'Talk to Lyra and use the 4 Seal Books to cleanse the tree. ""To be continued"".', 'Main', 'NotStarted', 'ElfForest', 12, 1, 5000, 5000, 0, 'Talk', 'Lyra', 'Origin Tree', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_lyra, q_id, 'Reward', 'You have collected all 4 Seal Books! The Origin Tree is saved. But this is not the end... To be continued.', 1, true);
END $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM ""PlayerQuests"";
DELETE FROM ""NPCDialogues"";
DELETE FROM ""Quests"";
DELETE FROM ""NPCs"";
");
        }
    }
}
