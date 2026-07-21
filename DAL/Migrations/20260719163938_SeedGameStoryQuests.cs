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
('Fa', 'A farmer collecting enchanted pumpkins.', 'QuestGiver', 'AutumnPumpkin', 6.08, -161.9, 2.5, true),
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
    npc_rowan2 INT; npc_tristan INT; npc_arthur INT; npc_fa INT;
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
    SELECT ""NPCId"" INTO npc_fa FROM ""NPCs"" WHERE ""Name"" = 'Fa' LIMIT 1;
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
    VALUES 
    (npc_rowan1, q_id, 'None', 'Ah, a new traveler. Welcome to the Elf Forest.', 1, true),
    (npc_rowan1, q_id, 'None', 'This forest has been peaceful for centuries, but recently, dark forces have begun to gather.', 2, true),
    (npc_rowan1, q_id, 'Quest', 'I need your help to protect this place. Come speak to me when you are ready to begin.', 3, true);

    -- Q2
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Gather White Flowers', 'Collect 3 White Flowers from the forest.', 'Main', 'NotStarted', 'ElfForest', 1, 3, 100, 80, 0, 'Collect', 'White Flower', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_rowan1, q_id, 'None', 'Before we can confront the darkness, we need to prepare some basic remedies.', 1, true),
    (npc_rowan1, q_id, 'None', 'The old willow clearing has some magical herbs we can use.', 2, true),
    (npc_rowan1, q_id, 'Quest', 'Please head over there and gather 3 White Flowers for me.', 3, true);

    -- Q3
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""RewardSkillId"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Deliver White Flowers', 'Deliver the gathered flowers to Elder Rowan.', 'Main', 'NotStarted', 'ElfForest', 1, 1, 50, 50, 0, (SELECT ""SkillId"" FROM ""Skills"" WHERE ""Name"" = 'Dark Poison Zone' LIMIT 1), 'Talk', 'Elder Rowan', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_rowan1, q_id, 'None', 'You have returned quickly. Did you find the flowers?', 1, true),
    (npc_rowan1, q_id, 'None', 'Excellent, these are in perfect condition. They will make fine healing poultices.', 2, true),
    (npc_rowan1, q_id, 'Reward', 'Thank you! Take this as a token of my appreciation.', 3, true);

    -- Q4
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Equip Your Skill', 'Equip your first combat skill.', 'Main', 'NotStarted', 'ElfForest', 1, 1, 100, 100, 0, 'EquipSkill', 'Skill Panel', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_rowan1, q_id, 'None', 'Now that you have your reward, it is time to learn how to defend yourself.', 1, true),
    (npc_rowan1, q_id, 'None', 'In this world, skills are essential for survival. You cannot fight with bare hands alone.', 2, true),
    (npc_rowan1, q_id, 'Quest', 'Open your Skill Panel and equip your first combat skill before you face real danger.', 3, true);

    -- Q5
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Defeat Slimes', 'Kill 3 SlimeLittle monsters in the forest.', 'Main', 'NotStarted', 'ElfForest', 1, 3, 150, 150, 0, 'Defeat', 'SlimeLittle', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_rowan1, q_id, 'None', 'Good, you are armed and ready. It is time to test your newfound abilities.', 1, true),
    (npc_rowan1, q_id, 'None', 'The outskirts of our forest have been overrun by strange, aggressive slimes.', 2, true),
    (npc_rowan1, q_id, 'Quest', 'Head out and defeat 3 SlimeLittle monsters to prove your worth to the village.', 3, true);

    -- Q6
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 1] The Swamp Demon', 'Slay the Swamp Demon and obtain its Seal Book.', 'Main', 'NotStarted', 'ElfForest', 2, 1, 250, 500, 0, 'Defeat', 'SwampDemon', 'Deep Woods', 2, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_rowan1, q_id, 'None', 'You handled those slimes well. But a much greater threat lurks in the deep woods.', 1, true),
    (npc_rowan1, q_id, 'None', 'A terrible Swamp Demon has made its lair there, corrupting the land with its presence.', 2, true),
    (npc_rowan1, q_id, 'Quest', 'You must destroy the Swamp Demon and claim the Swamp Seal Book it guards. We are counting on you!', 3, true);

    -- Q7
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] The Origin Tree', 'Talk to Lyra about the cursed Origin Tree and the 4 Seal Books.', 'Main', 'NotStarted', 'ElfForest', 2, 1, 100, 100, 0, 'Talk', 'Lyra', 'Origin Tree', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_lyra, q_id, 'None', 'Greetings, brave warrior. I am Lyra, the spirit of the Origin Tree.', 1, true),
    (npc_lyra, q_id, 'None', 'As you can see, the tree has been cursed and is slowly dying.', 2, true),
    (npc_lyra, q_id, 'Quest', 'Only the 4 Seal Books can cleanse it. You have one, but you must find the remaining three!', 3, true);

    -- Q8
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] The Mysterious Figure', 'Follow the cloaked figure through the portal to Autumn Pumpkin.', 'Main', 'NotStarted', 'ElfForest', 2, 1, 50, 50, 0, 'Explore', 'Portal', 'Elf Forest', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_mystery, q_id, 'None', 'Heh... So you are the one collecting the Seal Books?', 1, true),
    (npc_mystery, q_id, 'None', 'You know nothing of the true history of this world, or why the tree was cursed.', 2, true),
    (npc_mystery, q_id, 'Quest', 'If you want the truth, follow me through this portal. Don''t keep me waiting.', 3, true);

    -- MAP 2
    -- Q9
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] Where Are We?', 'Teleported onto the beach, proceed to the castle and ask Elder Rowan where this is. After introductions, realize you have no money and ask for work to buy food. Collect 8 Enchanted Pumpkins and hand them to farmer Fa, who will ask you to deliver them to the ruined city.', 'Main', 'NotStarted', 'AutumnPumpkin', 3, 8, 150, 200, 0, 'Collect', 'Enchanted Pumpkin', 'Pumpkin Town', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_rowan2, q_id, 'None', 'Welcome to the beach. We were teleported here by the portal.', 1, true),
    (npc_rowan2, q_id, 'None', 'You seem to have no money for food. Why don''t you look for some work?', 2, true),
    (npc_rowan2, q_id, 'Quest', 'Go talk to Fa, he needs help collecting 8 Enchanted Pumpkins.', 3, true);

    -- Q10
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] The Ruined City', 'Discover that the people in the castle have all been killed, return and report to guard Tristan and hand over the remaining pumpkins.', 'Main', 'NotStarted', 'AutumnPumpkin', 3, 1, 100, 100, 0, 'Talk', 'Tristan', 'City Gate', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_tristan, q_id, 'None', 'Halt! Who goes there? Ah, you brought pumpkins from Fa?', 1, true),
    (npc_tristan, q_id, 'None', 'I have terrible news. The people in the castle... they have all been massacred.', 2, true),
    (npc_tristan, q_id, 'Quest', 'This is a disaster. Please, we need your help to figure out what happened!', 3, true);

    -- Q11
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""RewardSkillId"", ""RewardItemId"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] The Silver Knight''s Training', 'Follow the guard''s words and find the silver paladin Arthur. Learn about his sealed power and train under him. Enter dungeons to level up. Receive DarkExplosion and Silver Necklace, then defeat 20 evil monsters.', 'Main', 'NotStarted', 'AutumnPumpkin', 12, 20, 300, 300, 0, (SELECT ""SkillId"" FROM ""Skills"" WHERE ""Name"" = 'DarkExplosion' LIMIT 1), (SELECT ""ItemId"" FROM ""Items"" WHERE ""Name"" = 'Silver Necklace' LIMIT 1), 'Defeat', 'Ghost/RobberAssassin/RedGuard/GoblinSpear/GoblinWarrior/RobberArcher/NecromancerCast', 'Ruined City', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_arthur, q_id, 'None', 'Greetings, warrior. I am Arthur, the silver knight.', 1, true),
    (npc_arthur, q_id, 'None', 'I suffered severe internal injuries and my power has been sealed away.', 2, true),
    (npc_arthur, q_id, 'None', 'I need someone to take my place. You must train in the dungeons to level up.', 3, true),
    (npc_arthur, q_id, 'Quest', 'Here, take this DarkExplosion skill and Silver Necklace. Now, defeat 20 monsters for me!', 4, true);

    -- Q12
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 2] Slay the Dragon', 'Turn in the quest and get Arthur''s recognition of your strength, receive quest to kill DragonBossIdle. Go kill dragon DragonBossIdle.', 'Main', 'NotStarted', 'AutumnPumpkin', 5, 1, 500, 1000, 0, 'Defeat', 'DragonBossIdle', 'Ruined City', 7, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_arthur, q_id, 'None', 'You have returned, and you are stronger than before.', 1, true),
    (npc_arthur, q_id, 'None', 'I recognize your true strength now. You are ready for the ultimate challenge.', 2, true),
    (npc_arthur, q_id, 'Quest', 'A terrible dragon threatens our existence. Go and slay the DragonBossIdle!', 3, true);

    -- Q13
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 2] The Frozen Threat', 'Talk to Arthur and receive the knight''s thanks, ask about the whereabouts of the ??? and he directs you to the frozen land devastated by the codex, go to Frozen Mountains.', 'Main', 'NotStarted', 'AutumnPumpkin', 5, 1, 100, 100, 0, 'Talk', 'Arthur', 'Ruined City', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_arthur, q_id, 'None', 'You did it! The dragon is slain. I cannot thank you enough.', 1, true),
    (npc_arthur, q_id, 'None', 'You ask about the mysterious figure? The one who did this?', 2, true),
    (npc_arthur, q_id, 'Quest', 'He went towards the frozen lands devastated by the codex. Head to the Frozen Mountains next.', 3, true);

    -- MAP 3
    -- Q14
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""RewardItemId"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] The Ice Slimes', 'Meet Queen Roselyn Aurora and defeat 8 Ice Slimes.', 'Main', 'NotStarted', 'FrozenMountain', 6, 8, 300, 300, 0, (SELECT ""ItemId"" FROM ""Items"" WHERE ""Name"" = 'Magic Flour' LIMIT 1), 'Defeat', 'slime_ice', 'Snow Fields', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_queen, q_id, 'None', 'Ah, a survivor from the ruins. I am Queen Roselyn Aurora.', 1, true),
    (npc_queen, q_id, 'None', 'This land is devastated by the codex. Only volunteers remain to defend it.', 2, true),
    (npc_queen, q_id, 'Quest', 'Please, clear out 8 ice slimes from the Snow Fields to help us.', 3, true);

    -- Q15
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] Magic Flour for the Priest', 'Deliver Magic Flour (obtained from the Queen) to the Priest (Zephyr).', 'Main', 'NotStarted', 'FrozenMountain', 6, 1, 150, 150, 0, 'Talk', 'Zephyr', 'Frozen Mountain', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_queen, q_id, 'None', 'Your efforts have not gone unnoticed. The slimes are thinning out.', 1, true),
    (npc_queen, q_id, 'None', 'However, our Priest Zephyr requires supplies for a ritual.', 2, true),
    (npc_queen, q_id, 'Quest', 'Take this Magic Flour and deliver it to him at the mountain peak.', 3, true);

    -- Q16
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] Dragons of Snow', 'Meet Zephyr and slay 5 Ice Dragons on the mountain.', 'Main', 'NotStarted', 'FrozenMountain', 7, 5, 400, 400, 0, 'Defeat', 'Ice_Dragon', 'Frozen Mountain', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_zephyr, q_id, 'None', 'Ah, the flour from the Queen! Thank you, traveler.', 1, true),
    (npc_zephyr, q_id, 'None', 'The codex has warped the creatures here. The beasts have become feral and dangerous.', 2, true),
    (npc_zephyr, q_id, 'Quest', 'To secure our borders, go slay 5 Ice Dragons on the mountain.', 3, true);

    -- Q17
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 3] The Forbidden Zone', 'Head to the forbidden zone and speak with Roland to explore it.', 'Main', 'NotStarted', 'FrozenMountain', 7, 1, 150, 150, 0, 'Explore', 'Roland', 'Forbidden Zone', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_roland, q_id, 'None', 'Halt! This is the forbidden zone. None may enter.', 1, true),
    (npc_roland, q_id, 'None', 'Wait... you have the aura of one who has fought the Ice Dragons.', 2, true),
    (npc_roland, q_id, 'Quest', 'Since you made it this far, help me explore this dangerous area.', 3, true);

    -- Q18
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 3] Truth of the Codex', 'Discover the truth of the codex and defeat GolemBoss to get the Golem Seal Book.', 'Main', 'NotStarted', 'FrozenMountain', 8, 1, 800, 1500, 0, 'Defeat', 'GolemBoss', 'Forbidden Zone', 10, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_roland, q_id, 'None', 'We have uncovered the origin of the codex... The truth is terrifying.', 1, true),
    (npc_roland, q_id, 'None', 'A massive ancient golem guards the final piece of the puzzle.', 2, true),
    (npc_roland, q_id, 'Quest', 'Defeat the giant GolemBoss to claim the Golem Seal Book! Do not fail us.', 3, true);

    -- MAP 4
    -- Q19
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Skeleton Army', 'Defeat 12 skeletons in the valley for Valiant Warrior.', 'Main', 'NotStarted', 'AbandonedCastle', 9, 12, 500, 500, 0, 'Defeat', 'Skeleton', 'Valley', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_valiant, q_id, 'None', 'Stay back! The undead are relentless today.', 1, true),
    (npc_valiant, q_id, 'None', 'An ancient power is leaking, causing skeletons to multiply out of control.', 2, true),
    (npc_valiant, q_id, 'Quest', 'I can''t hold them off alone. Defeat 12 of them in the valley!', 3, true);

    -- Q20
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] The Abandoned Village', 'Go to Tide-Knell village, meet Natalie, and dig up the skull near the old well.', 'Main', 'NotStarted', 'AbandonedCastle', 9, 1, 300, 300, 0, 'Interact', 'Skull', 'Tide-Knell', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES (npc_valiant, q_id, 'Quest', 'The animals are fleeing from the abandoned village Tide-Knell. Investigate it and find Natalie.', 1, true);
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_natalie, q_id, 'None', 'Are you here to help me? I cannot leave this place...', 2, true),
    (npc_natalie, q_id, 'Quest', 'Please... dig up what is buried under the small tree near the old well.', 3, true);

    -- Q21
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""RewardItemId"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Rest in Peace', 'Read Natalie''s suicide letter and bury her remains under the ivy tree. Receive Mystic Key.', 'Main', 'NotStarted', 'AbandonedCastle', 10, 1, 400, 400, 0, (SELECT ""ItemId"" FROM ""Items"" WHERE ""Name"" = 'Mystic Key' LIMIT 1), 'Talk', 'Natalie', 'Tide-Knell', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_natalie, q_id, 'None', 'Thank you for finding my remains. Now I can finally rest in peace.', 1, true),
    (npc_natalie, q_id, 'None', 'The ancient power leak was my doing. I am so sorry for the chaos.', 2, true),
    (npc_natalie, q_id, 'Reward', 'Take this key. It will unlock the doors to the island castle. Farewell.', 3, true);

    -- Q22
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Deserted Island', 'Talk to Elf Guard on the deserted island and collect 5 Ancient Leaves.', 'Main', 'NotStarted', 'AbandonedCastle', 10, 5, 450, 450, 0, 'Collect', 'Ancient Leaves', 'Northern Plateau', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_elfguard, q_id, 'None', 'You actually survived the waves and made it to this deserted island.', 1, true),
    (npc_elfguard, q_id, 'None', 'I need your assistance to prepare a ritual of return.', 2, true),
    (npc_elfguard, q_id, 'Quest', 'Help me collect 5 Ancient Leaves from the Northern Plateau.', 3, true);

    -- Q23
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""BossMonsterId"", ""IsActive"")
    VALUES ('[Chapter 4] The UnderKing', 'Defeat the UnderKing to claim the final UnderKing Seal Book.', 'Main', 'NotStarted', 'AbandonedCastle', 11, 1, 2000, 3000, 0, 'Defeat', 'UnderKing', 'Deserted Island', 15, true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_elfguard, q_id, 'None', 'We have everything we need. But a dark presence blocks our path.', 1, true),
    (npc_elfguard, q_id, 'None', 'The UnderKing himself has awakened, and he guards the final Seal Book.', 2, true),
    (npc_elfguard, q_id, 'Quest', 'You must end his reign! Defeat the UnderKing and claim the book!', 3, true);

    -- Q24
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 4] Return to Elf Forest', 'Talk to Elf Guard. He will open a portal back to the Elf Forest.', 'Main', 'NotStarted', 'AbandonedCastle', 12, 1, 100, 100, 0, 'Talk', 'Elf Guard', 'Deserted Island', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_elfguard, q_id, 'None', 'It is done. The UnderKing is defeated, and you have all 4 Seal Books.', 1, true),
    (npc_elfguard, q_id, 'None', 'The fate of the Origin Tree now rests entirely in your hands.', 2, true),
    (npc_elfguard, q_id, 'Reward', 'Farewell, hero. I will use my power to open a portal back to the Elf Forest. Save the tree!', 3, true);

    -- Q25
    INSERT INTO ""Quests"" (""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""IsActive"")
    VALUES ('[Chapter 1] Save the Origin Tree', 'Talk to Lyra and use the 4 Seal Books to cleanse the tree. ""To be continued"".', 'Main', 'NotStarted', 'ElfForest', 12, 1, 5000, 5000, 0, 'Talk', 'Lyra', 'Origin Tree', true) RETURNING ""QuestId"" INTO q_id;
    INSERT INTO ""NPCDialogues"" (""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"")
    VALUES 
    (npc_lyra, q_id, 'None', 'You have returned! And I can sense the power of the 4 Seal Books.', 1, true),
    (npc_lyra, q_id, 'None', 'The curse is breaking... The Origin Tree is finally healing!', 2, true),
    (npc_lyra, q_id, 'Reward', 'Thank you! The Origin Tree is saved. But this is not the end... To be continued.', 3, true);
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
