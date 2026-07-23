using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllGameDataToSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ─────────────────────────────────────────────────────────────────────────
            // Idempotent seed: ON CONFLICT DO NOTHING ensures this migration is safe
            // on both fresh DBs and existing ones that already ran the old SQL migrations.
            // ─────────────────────────────────────────────────────────────────────────
            migrationBuilder.Sql(@"
-- Items (ID 1-33)
INSERT INTO ""Items"" (""ItemId"", ""Name"", ""Description"", ""Type"", ""Rarity"", ""Slot"", ""BaseValue"", ""MaxStack"", ""IsActive"", ""CorruptionReduction"", ""CreatedAt"") VALUES
(1,  'Gold',                 'In-game gold currency.',                                                                      'Currency',   'Common',    'None',     1,    2147483647, true, 0, '2024-01-01T00:00:00Z'),
(2,  'Exp',                  'Experience points for leveling up.',                                                          'Currency',   'Common',    'None',     1,    2147483647, true, 0, '2024-01-01T00:00:00Z'),
(3,  'Gem',                  'Premium gem used to purchase high-tier items.',                                               'Currency',   'Rare',      'None',     5,    2147483647, true, 0, '2024-01-01T00:00:00Z'),
(4,  'Lucky Ticket',         'Lucky ticket used to spin the gacha banner.',                                                 'Consumable', 'Rare',      'None',     1,    99,         true, 0, '2024-01-01T00:00:00Z'),
(5,  'Iron Sword',           'Basic iron sword for beginner warriors.',                                                     'Weapon',     'Common',    'Weapon',   150,  1,          true, 0, '2024-01-01T00:00:00Z'),
(6,  'Hunter Bow',           'A forest hunter bow, light and accurate.',                                                    'Weapon',     'Common',    'Weapon',   150,  1,          true, 0, '2024-01-01T00:00:00Z'),
(7,  'Apprentice Staff',     'A novice magic staff for casting light spells.',                                              'Weapon',     'Common',    'Weapon',   150,  1,          true, 0, '2024-01-01T00:00:00Z'),
(8,  'Elven Blade',          'A glowing elven blade, forged deep in the ancient forest.',                                   'Weapon',     'Epic',      'Weapon',   800,  1,          true, 0, '2024-01-01T00:00:00Z'),
(9,  'Leather Armor',        'Light leather armor that provides basic defense.',                                            'Armor',      'Common',    'Armor',    120,  1,          true, 0, '2024-01-01T00:00:00Z'),
(10, 'Iron Helmet',          'Sturdy iron helmet that protects the head from damage.',                                      'Armor',      'Common',    'Helmet',   100,  1,          true, 0, '2024-01-01T00:00:00Z'),
(11, 'Wind Boots',           'Wind-infused boots that increase movement speed.',                                            'Armor',      'Uncommon',  'Boots',    200,  1,          true, 0, '2024-01-01T00:00:00Z'),
(12, 'Dragon Scale Armor',   'Legendary dragon scale armor offering supreme defense.',                                      'Armor',      'Legendary', 'Armor',    2000, 1,          true, 0, '2024-01-01T00:00:00Z'),
(13, 'Phantom Cloak',        'Shadow cloak that boosts speed and evasion.',                                                 'Armor',      'Epic',      'Armor',    900,  1,          true, 0, '2024-01-01T00:00:00Z'),
(14, 'Shadow Hood',          'Dark hood that increases critical strike damage.',                                            'Armor',      'Rare',      'Helmet',   500,  1,          true, 0, '2024-01-01T00:00:00Z'),
(15, 'Iron Gauntlets',       'Iron gauntlets that increase physical damage.',                                               'Armor',      'Common',    'Gloves',   120,  1,          true, 0, '2024-01-01T00:00:00Z'),
(16, 'Leather Gauntlets',    'Soft leather gauntlets that allow flexible combat.',                                          'Armor',      'Common',    'Gloves',   100,  1,          true, 0, '2024-01-01T00:00:00Z'),
(17, 'Copper Ring',          'Basic copper ring that slightly boosts stats.',                                               'Armor',      'Common',    'Ring',     80,   1,          true, 0, '2024-01-01T00:00:00Z'),
(18, 'Silver Necklace',      'Silver necklace that increases maximum energy.',                                              'Armor',      'Uncommon',  'Necklace', 200,  1,          true, 0, '2024-01-01T00:00:00Z'),
(19, 'Small Health Potion',  'Small health potion that restores 80 HP.',                                                    'Consumable', 'Common',    'None',     30,   99,         true, 0, '2024-01-01T00:00:00Z'),
(20, 'Large Health Potion',  'Large health potion that restores 200 HP.',                                                   'Consumable', 'Uncommon',  'None',     80,   99,         true, 0, '2024-01-01T00:00:00Z'),
(21, 'Energy Elixir',        'Energy elixir that restores 60 Energy.',                                                     'Consumable', 'Uncommon',  'None',     60,   99,         true, 0, '2024-01-01T00:00:00Z'),
(22, 'Skill Upgrade Stone',  'Magic stone used to upgrade player skills.',                                                  'Material',   'Rare',      'None',     50,   999,        true, 0, '2024-01-01T00:00:00Z'),
(23, 'White Flower',         'White flower collected in the fairy forest.',                                                 'QuestItem',  'Common',    'None',     0,    99,         true, 0, '2024-01-01T00:00:00Z'),
(24, 'Wood Logs',            'Logs collected from the ancient forest.',                                                     'QuestItem',  'Common',    'None',     0,    99,         true, 0, '2024-01-01T00:00:00Z'),
(25, 'Ancient Leaves',       'Ancient tree leaves collected from the fairy forest.',                                        'QuestItem',  'Common',    'None',     0,    99,         true, 0, '2024-01-01T00:00:00Z'),
(26, 'Dragon Seal Book',     'Dragon Seal Book. Dropped by DragonBossIdle. Collect all 4 seal books to save the World Tree.','QuestItem', 'Epic',     'None',     0,    1,          true, 0, '2024-01-01T00:00:00Z'),
(27, 'Golem Seal Book',      'Golem Seal Book. Dropped by GolemBoss.',                                                     'QuestItem',  'Epic',      'None',     0,    1,          true, 0, '2024-01-01T00:00:00Z'),
(28, 'UnderKing Seal Book',  'UnderKing Seal Book. Dropped by the UnderKing boss.',                                        'QuestItem',  'Epic',      'None',     0,    1,          true, 0, '2024-01-01T00:00:00Z'),
(29, 'Swamp Seal Book',      'Swamp Demon Seal Book. Dropped by SwampDemon boss.',                                         'QuestItem',  'Epic',      'None',     0,    1,          true, 0, '2024-01-01T00:00:00Z'),
(30, 'Enchanted Pumpkin',    'A magical pumpkin glowing with autumn energy.',                                               'QuestItem',  'Common',    'None',     0,    99,         true, 0, '2024-01-01T00:00:00Z'),
(31, 'Magic Flour',          'Mystical flour used for special spells.',                                                     'QuestItem',  'Common',    'None',     0,    99,         true, 0, '2024-01-01T00:00:00Z'),
(32, 'Spirit Skull',         'A skull radiating with ghostly presence.',                                                   'QuestItem',  'Common',    'None',     0,    99,         true, 0, '2024-01-01T00:00:00Z'),
(33, 'Mystic Key',           'A key that opens the castle on the deserted island.',                                        'QuestItem',  'Epic',      'None',     0,    1,          true, 0, '2024-01-01T00:00:00Z')
ON CONFLICT DO NOTHING;
");

            migrationBuilder.Sql(@"
-- EquipmentStats for system items (ID = ItemId)
INSERT INTO ""EquipmentStats"" (""EquipmentStatsId"", ""ItemId"", ""BaseHp"", ""BaseAtk"", ""BaseDef"", ""BonusHp"", ""BonusAtk"", ""BonusDef"", ""BonusMoveSpeed"", ""BonusAttackSpeed"", ""BonusCritRate"", ""BonusCritDamage"", ""BonusDamageBonus"") VALUES
(5,  5,  0,   35,  0,   0,   8,  0,  0,  0,  30, 50,  0),
(6,  6,  0,   30,  0,   0,   6,  0,  0,  10, 40, 30,  0),
(7,  7,  0,   28,  0,   0,   5,  0,  0,  0,  20, 80,  10),
(8,  8,  0,   80,  0,   0,   20, 0,  0,  5,  60, 100, 15),
(9,  9,  50,  0,   12,  10,  0,  3,  0,  0,  0,  0,   0),
(10, 10, 100, 0,   30,  20,  0,  8,  0,  0,  0,  0,   0),
(11, 11, 0,   0,   5,   0,   0,  2,  20, 0,  0,  0,   0),
(12, 12, 500, 0,   120, 100, 0,  30, 0,  0,  0,  0,   0),
(13, 13, 0,   0,   60,  0,   0,  15, 15, 0,  0,  0,   0),
(14, 14, 0,   0,   20,  0,   0,  5,  0,  0,  80, 120, 0),
(15, 15, 0,   20,  5,   0,   5,  2,  0,  0,  0,  0,   5),
(16, 16, 0,   15,  3,   0,   3,  1,  5,  5,  0,  0,   0),
(17, 17, 30,  5,   3,   5,   2,  1,  0,  0,  10, 10,  0),
(18, 18, 80,  0,   5,   20,  0,  2,  0,  0,  0,  0,   0)
ON CONFLICT DO NOTHING;
");

            migrationBuilder.Sql(@"
-- NPCs (ID 1-13)
INSERT INTO ""NPCs"" (""NPCId"", ""Name"", ""Description"", ""Type"", ""MapName"", ""PositionX"", ""PositionY"", ""InteractionRadius"", ""IsActive"") VALUES
(1,  'Elder Rowan',           'The wise guide of the Elf Forest.',              'QuestGiver', 'ElfForest',      12.4932,     18.61223,     2.5, true),
(2,  'Lyra',                  'A spirit of the forest.',                        'QuestGiver', 'ElfForest',      41.94587,   -27.18052,     2.5, true),
(3,  'Mysterious Figure',     'A mysterious figure in a cloak.',               'QuestGiver', 'ElfForest',      10.11194,   -45.86301,     2.5, true),
(4,  'Elder Rowan (Pumpkin)', 'The wise guide, now in the pumpkin town.',      'QuestGiver', 'AutumnPumpkin',   1.873512,   -92.8158,      2.5, true),
(5,  'Tristan',               'The city gate guard.',                          'QuestGiver', 'AutumnPumpkin',  11.62283,  -113.6158,      2.5, true),
(6,  'Arthur',                'The silver knight.',                            'QuestGiver', 'AutumnPumpkin',  77.54412,   -77.44301,     2.5, true),
(7,  'Fa',                    'A farmer collecting enchanted pumpkins.',       'QuestGiver', 'AutumnPumpkin',   6.08,      -161.9,         2.5, true),
(8,  'Roselyn Aurora Queen',  'Queen of the frozen lands.',                    'QuestGiver', 'FrozenMountain', 160.8554,   -35.6486,      2.5, true),
(9,  'Zephyr',                'The witch and disguised priest.',               'QuestGiver', 'FrozenMountain',  6.996814,   -0.2094555,   2.5, true),
(10, 'Roland',                'The forbidden zone guard.',                     'QuestGiver', 'FrozenMountain', 70.45686,   18.80354,      2.5, true),
(11, 'Valiant Warrior',       'A brave warrior fighting skeletons.',           'QuestGiver', 'AbandonedCastle',-10.66112,  54.92884,      2.5, true),
(12, 'Natalie',               'The ghost of a young girl.',                    'QuestGiver', 'AbandonedCastle',-48.92126, -21.12006,      2.5, true),
(13, 'Elf Guard',             'The lone guard of the deserted island.',        'QuestGiver', 'AbandonedCastle', -6.237758, -13.13438,     2.5, true)
ON CONFLICT DO NOTHING;
");

            migrationBuilder.Sql(@"
-- Quests (ID 1-29) with final EXP/Gold/Gems after all migration adjustments
INSERT INTO ""Quests"" (""QuestId"", ""Title"", ""Description"", ""Type"", ""DefaultStatus"", ""MapName"", ""RequiredLevel"", ""TargetAmount"", ""RewardExperience"", ""RewardGold"", ""RewardGems"", ""ObjectiveType"", ""ObjectiveTarget"", ""ObjectiveLocation"", ""QuestGiverName"", ""IsActive"", ""BossMonsterId"", ""RewardSkillId"", ""RewardItemId"") VALUES
(1,  '[Chapter 1] Speak with Elder Rowan',      'Talk to Elder Rowan in the Elf Forest.',                                                                                                                      'Main','NotStarted','ElfForest',      1,  1,  5,   10,  5,  'Talk',       'Elder Rowan',    'Elf Forest',       'Elder Rowan',          true, NULL, NULL, NULL),
(2,  '[Chapter 1] Gather White Flowers',         'Collect 3 White Flowers from the forest.',                                                                                                                   'Main','NotStarted','ElfForest',      1,  3,  10,  8,   5,  'Collect',    'White Flower',   'Elf Forest',       'Elder Rowan',          true, NULL, NULL, NULL),
(3,  '[Chapter 1] Deliver White Flowers',        'Deliver the gathered flowers to Elder Rowan.',                                                                                                               'Main','NotStarted','ElfForest',      1,  1,  5,   5,   5,  'Talk',       'Elder Rowan',    'Elf Forest',       'Elder Rowan',          true, NULL, 10,   NULL),
(4,  '[Chapter 1] Equip Your Skill',             'Equip your first combat skill.',                                                                                                                             'Main','NotStarted','ElfForest',      1,  1,  10,  10,  5,  'EquipSkill', 'Skill Panel',    'Elf Forest',       'Elder Rowan',          true, NULL, NULL, NULL),
(5,  '[Chapter 1] Defeat Slimes',                'Kill 3 SlimeLittle monsters in the forest.',                                                                                                                'Main','NotStarted','ElfForest',      1,  3,  15,  15,  5,  'Defeat',     'SlimeLittle',    'Elf Forest',       'Elder Rowan',          true, NULL, NULL, NULL),
(6,  '[Chapter 1] The Swamp Demon',              'Slay the Swamp Demon and obtain its Seal Book.',                                                                                                            'Main','NotStarted','ElfForest',      2,  1,  25,  50,  5,  'Defeat',     'SwampDemon',     'Deep Woods',       'Elder Rowan',          true, 2,    NULL, NULL),
(7,  '[Chapter 1] The Origin Tree',              'Talk to Lyra about the cursed Origin Tree and the 4 Seal Books.',                                                                                           'Main','NotStarted','ElfForest',      2,  1,  10,  10,  5,  'Talk',       'Lyra',           'Origin Tree',      'Lyra',                 true, NULL, NULL, NULL),
(8,  '[Chapter 1] The Mysterious Figure',        'Follow the cloaked figure through the portal to Autumn Pumpkin.',                                                                                           'Main','NotStarted','ElfForest',      2,  1,  5,   5,   5,  'Explore',    'Portal',         'Elf Forest',       'Mysterious Figure',    true, NULL, NULL, NULL),
(9,  '[Chapter 2] Where Are We?',               'Teleported onto the beach, proceed to the castle and ask Elder Rowan where this is. After introductions, realize you have no money and ask if there is work to earn food.','Main','NotStarted','AutumnPumpkin',3,1,100, 5,   5,  'Talk',       'Elder Rowan',    'Autumn Pumpkin',   'Elder Rowan',          true, NULL, NULL, NULL),
(10, '[Chapter 2] Work for Food',               'Collect 8 Enchanted Pumpkins from the field and hand them over to farmer Fa.',                                                                               'Main','NotStarted','AutumnPumpkin',3,  8,  300, 10,  5,  'Collect',    'Enchanted Pumpkin','Pumpkin Town',   'Fa',                   true, NULL, NULL, NULL),
(11, '[Chapter 2] Delivery to the City',        'Help Fa deliver the harvested pumpkins to guard Tristan at the ruined city gate.',                                                                           'Main','NotStarted','AutumnPumpkin',3,  1,  200, 5,   5,  'Talk',       'Tristan',        'City Gate',        'Fa',                   true, NULL, NULL, NULL),
(12, '[Chapter 2] The Ruined City',             'Enter the city and investigate the dead bodies, then report back to guard Tristan.',                                                                          'Main','NotStarted','AutumnPumpkin',3,  5,  250, 5,   5,  'Interact',   'Corpse',         'Ruined City',      'Tristan',              true, NULL, NULL, NULL),
(13, '[Chapter 2] Seek the Silver Knight',      'Report the massacre to Tristan. He asks you to find the silver knight Arthur for help.',                                                                      'Main','NotStarted','AutumnPumpkin',3,  1,  250, 5,   5,  'Talk',       'Arthur',         'Ruined City',      'Tristan',              true, NULL, NULL, NULL),
(14, '[Chapter 2] The Silver Knight''s Training','''Speak with Arthur and learn about his internal injuries and sealed power. Enter Dungeon ID 2 to train and level up your strength.',                       'Main','NotStarted','AutumnPumpkin',12, 1,  15,  15,  5,  'Explore',    'Dungeon_2',      'Dungeon',          'Arthur',               true, NULL, 9,    18),
(15, '[Chapter 2] Defeat the Evil Monsters',    'Receive the DarkExplosion skill and Silver Necklace from Arthur. Take his place to defeat 10 evil monsters in the Ruined City.',                             'Main','NotStarted','AutumnPumpkin',12, 10, 20,  20,  5,  'Defeat',     'Ghost/RobberAssassin/RedGuard/GoblinSpear/GoblinWarrior/RobberArcher/NecromancerCast','Ruined City','Arthur',true,NULL,NULL,NULL),
(16, '[Chapter 2] Slay the Dragon',             'Turn in the quest and get Arthur''s recognition of your strength, receive quest to kill DragonBossIdle. Go kill dragon DragonBossIdle.',                     'Main','NotStarted','AutumnPumpkin',5,  1,  50,  100, 5,  'Defeat',     'DragonBossIdle', 'Ruined City',      'Arthur',               true, 7,    NULL, NULL),
(17, '[Chapter 2] The Frozen Threat',           'Talk to Arthur and receive the knight''s thanks, ask about the whereabouts of the ??? and he directs you to the frozen land devastated by the codex.',       'Main','NotStarted','AutumnPumpkin',5,  1,  10,  10,  5,  'Talk',       'Arthur',         'Ruined City',      'Arthur',               true, NULL, NULL, NULL),
(18, '[Chapter 3] The Ice Slimes',              'Meet Queen Roselyn Aurora and defeat 8 Ice Slimes.',                                                                                                         'Main','NotStarted','FrozenMountain',6,  8,  30,  30,  5,  'Defeat',     'slime_ice',      'Snow Fields',      'Roselyn Aurora Queen', true, NULL, NULL, 31),
(19, '[Chapter 3] Magic Flour for the Priest',  'Deliver Magic Flour (obtained from the Queen) to the Priest (Zephyr).',                                                                                      'Main','NotStarted','FrozenMountain',6,  1,  15,  15,  5,  'Talk',       'Zephyr',         'Frozen Mountain',  'Roselyn Aurora Queen', true, NULL, NULL, NULL),
(20, '[Chapter 3] Dragons of Snow',             'Meet Zephyr and slay 5 Ice Dragons on the mountain.',                                                                                                        'Main','NotStarted','FrozenMountain',7,  5,  40,  40,  5,  'Defeat',     'Ice_Dragon',     'Frozen Mountain',  'Zephyr',               true, NULL, NULL, NULL),
(21, '[Chapter 3] The Forbidden Zone',          'Head to the forbidden zone and speak with Roland to explore it.',                                                                                             'Main','NotStarted','FrozenMountain',7,  1,  15,  15,  5,  'Explore',    'Roland',         'Forbidden Zone',   'Roland',               true, NULL, NULL, NULL),
(22, '[Chapter 3] Truth of the Codex',          'Discover the truth of the codex and defeat GolemBoss to get the Golem Seal Book.',                                                                           'Main','NotStarted','FrozenMountain',8,  1,  80,  150, 5,  'Defeat',     'GolemBoss',      'Forbidden Zone',   'Roland',               true, 10,   NULL, NULL),
(23, '[Chapter 4] Skeleton Army',               'Defeat 12 skeletons in the valley for Valiant Warrior.',                                                                                                     'Main','NotStarted','AbandonedCastle',9,  12, 50,  50,  5,  'Defeat',     'Skeleton',       'Valley',           'Valiant Warrior',      true, NULL, NULL, NULL),
(24, '[Chapter 4] The Abandoned Village',       'Go to Tide-Knell village, meet Natalie, and dig up the skull near the old well.',                                                                            'Main','NotStarted','AbandonedCastle',9,  1,  30,  30,  5,  'Interact',   'Skull',          'Tide-Knell',       'Valiant Warrior',      true, NULL, NULL, NULL),
(25, '[Chapter 4] Rest in Peace',               'Read Natalie''s suicide letter and bury her remains under the ivy tree. Receive Mystic Key.',                                                                'Main','NotStarted','AbandonedCastle',10, 1,  40,  40,  5,  'Talk',       'Natalie',        'Tide-Knell',       'Natalie',              true, NULL, NULL, 33),
(26, '[Chapter 4] Deserted Island',             'Talk to Elf Guard on the deserted island and collect 5 Ancient Leaves.',                                                                                     'Main','NotStarted','AbandonedCastle',10, 5,  45,  45,  5,  'Collect',    'Ancient Leaves', 'Northern Plateau', 'Elf Guard',            true, NULL, NULL, NULL),
(27, '[Chapter 4] The UnderKing',               'Defeat the UnderKing to claim the final UnderKing Seal Book.',                                                                                               'Main','NotStarted','AbandonedCastle',11, 1,  200, 300, 5,  'Defeat',     'UnderKing',      'Deserted Island',  'Elf Guard',            true, 15,   NULL, NULL),
(28, '[Chapter 4] Return to Elf Forest',        'Talk to Elf Guard. He will open a portal back to the Elf Forest.',                                                                                           'Main','NotStarted','AbandonedCastle',12, 1,  10,  10,  5,  'Talk',       'Elf Guard',      'Deserted Island',  'Elf Guard',            true, NULL, NULL, NULL),
(29, '[Chapter 1] Save the Origin Tree',        'Talk to Lyra and use the 4 Seal Books to cleanse the tree. ""To be continued"".',                                                                            'Main','NotStarted','ElfForest',      12, 1,  500, 500, 5,  'Talk',       'Lyra',           'Origin Tree',      'Lyra',                 true, NULL, NULL, NULL)
ON CONFLICT DO NOTHING;
");

            migrationBuilder.Sql(@"
-- NPC Dialogues (ID 1-87)
INSERT INTO ""NPCDialogues"" (""NPCDialogueId"", ""NPCId"", ""LinkedQuestId"", ""ResponseType"", ""Content"", ""DisplayOrder"", ""IsActive"") VALUES
-- Q1
(1,  1, 1,  'None',   'Ah, a new traveler. Welcome to the Elf Forest.',                                                                  1, true),
(2,  1, 1,  'None',   'This forest has been peaceful for centuries, but recently, dark forces have begun to gather.',                     2, true),
(3,  1, 1,  'Quest',  'I need your help to protect this place. Come speak to me when you are ready to begin.',                           3, true),
-- Q2
(4,  1, 2,  'None',   'Before we can confront the darkness, we need to prepare some basic remedies.',                                    1, true),
(5,  1, 2,  'None',   'The old willow clearing has some magical herbs we can use.',                                                      2, true),
(6,  1, 2,  'Quest',  'Please head over there and gather 3 White Flowers for me.',                                                       3, true),
-- Q3
(7,  1, 3,  'None',   'You have returned quickly. Did you find the flowers?',                                                            1, true),
(8,  1, 3,  'None',   'Excellent, these are in perfect condition. They will make fine healing poultices.',                               2, true),
(9,  1, 3,  'Reward', 'Thank you! Take this as a token of my appreciation.',                                                             3, true),
-- Q4
(10, 1, 4,  'None',   'Now that you have your reward, it is time to learn how to defend yourself.',                                      1, true),
(11, 1, 4,  'None',   'In this world, skills are essential for survival. You cannot fight with bare hands alone.',                       2, true),
(12, 1, 4,  'Quest',  'Open your Skill Panel and equip your first combat skill before you face real danger.',                            3, true),
-- Q5
(13, 1, 5,  'None',   'Good, you are armed and ready. It is time to test your newfound abilities.',                                      1, true),
(14, 1, 5,  'None',   'The outskirts of our forest have been overrun by strange, aggressive slimes.',                                    2, true),
(15, 1, 5,  'Quest',  'Head out and defeat 3 SlimeLittle monsters to prove your worth to the village.',                                  3, true),
-- Q6
(16, 1, 6,  'None',   'You handled those slimes well. But a much greater threat lurks in the deep woods.',                               1, true),
(17, 1, 6,  'None',   'A terrible Swamp Demon has made its lair there, corrupting the land with its presence.',                         2, true),
(18, 1, 6,  'Quest',  'You must destroy the Swamp Demon and claim the Swamp Seal Book it guards. We are counting on you!',              3, true),
-- Q7
(19, 2, 7,  'None',   'Greetings, brave warrior. I am Lyra, the spirit of the Origin Tree.',                                            1, true),
(20, 2, 7,  'None',   'As you can see, the tree has been cursed and is slowly dying.',                                                   2, true),
(21, 2, 7,  'Quest',  'Only the 4 Seal Books can cleanse it. You have one, but you must find the remaining three!',                     3, true),
-- Q8
(22, 3, 8,  'None',   'Heh... So you are the one collecting the Seal Books?',                                                           1, true),
(23, 3, 8,  'None',   'You know nothing of the true history of this world, or why the tree was cursed.',                                2, true),
(24, 3, 8,  'Quest',  'If you want the truth, follow me through this portal. Don''t keep me waiting.',                                  3, true),
-- Q9
(25, 4, 9,  'None',   'Welcome to the beach. We were teleported here by the portal.',                                                   1, true),
(26, 4, 9,  'None',   'You seem to have no money for food. Why don''t you look for some work?',                                         2, true),
(27, 4, 9,  'Quest',  'Go talk to Fa, he is nearby and might need some help.',                                                          3, true),
-- Q10
(28, 7, 10, 'None',   'Ah, Elder Rowan sent you? Good timing.',                                                                         1, true),
(29, 7, 10, 'None',   'I need someone to help me harvest the fields.',                                                                  2, true),
(30, 7, 10, 'Quest',  'Please collect 8 Enchanted Pumpkins for me.',                                                                    3, true),
-- Q11
(31, 7, 11, 'None',   'Great job with the pumpkins! You are a hard worker.',                                                            1, true),
(32, 7, 11, 'None',   'Now, I need these delivered to the city gate.',                                                                  2, true),
(33, 7, 11, 'Quest',  'Please take them to guard Tristan at the ruined city.',                                                          3, true),
-- Q12
(34, 5, 12, 'None',   'Halt! Who goes there? Ah, you brought pumpkins from Fa?',                                                        1, true),
(35, 5, 12, 'None',   'Something is wrong in the city... It is too quiet.',                                                             2, true),
(36, 5, 12, 'Quest',  'Please go inside and investigate. Let me know if you find anything suspicious.',                                 3, true),
-- Q13
(37, 5, 13, 'None',   'What?! The people inside have all been massacred? Corpses everywhere?',                                          1, true),
(38, 5, 13, 'None',   'This is a disaster. We need someone strong to handle this.',                                                     2, true),
(39, 5, 13, 'Quest',  'Please, go find the silver knight Arthur and report this!',                                                      3, true),
-- Q14
(40, 6, 14, 'None',   'Greetings, warrior. I am Arthur, the silver knight.',                                                            1, true),
(41, 6, 14, 'None',   'I suffered severe internal injuries and my power has been sealed away.',                                         2, true),
(42, 6, 14, 'Quest',  'You must train in Dungeon 2 to level up and unlock your true potential. Go!',                                    3, true),
-- Q15
(43, 6, 15, 'None',   'Splendid! You have trained well and cleared the dungeon.',                                                       1, true),
(44, 6, 15, 'None',   'As promised, take this DarkExplosion skill and Silver Necklace.',                                                2, true),
(45, 6, 15, 'Quest',  'Now, use your power to defeat 10 evil monsters in the Ruined City!',                                             3, true),
-- Q16
(46, 6, 16, 'None',   'You have returned, and you are stronger than before.',                                                           1, true),
(47, 6, 16, 'None',   'I recognize your true strength now. You are ready for the ultimate challenge.',                                  2, true),
(48, 6, 16, 'Quest',  'A terrible dragon threatens our existence. Go and slay the DragonBossIdle!',                                     3, true),
-- Q17
(49, 6, 17, 'None',   'You did it! The dragon is slain. I cannot thank you enough.',                                                    1, true),
(50, 6, 17, 'None',   'You ask about the mysterious figure? The one who did this?',                                                     2, true),
(51, 6, 17, 'Quest',  'He went towards the frozen lands devastated by the codex. Head to the Frozen Mountains next.',                  3, true),
-- Q18
(52, 8, 18, 'None',   'Ah, a survivor from the ruins. I am Queen Roselyn Aurora.',                                                      1, true),
(53, 8, 18, 'None',   'This land is devastated by the codex. Only volunteers remain to defend it.',                                     2, true),
(54, 8, 18, 'Quest',  'Please, clear out 8 ice slimes from the Snow Fields to help us.',                                                3, true),
-- Q19
(55, 8, 19, 'None',   'Your efforts have not gone unnoticed. The slimes are thinning out.',                                             1, true),
(56, 8, 19, 'None',   'However, our Priest Zephyr requires supplies for a ritual.',                                                     2, true),
(57, 8, 19, 'Quest',  'Take this Magic Flour and deliver it to him at the mountain peak.',                                              3, true),
(58, 9, 19, 'None',   'Ah, the flour from the Queen! Thank you, traveler.',                                                             1, true),
-- Q20
(59, 9, 20, 'None',   'The codex has warped the creatures here. The beasts have become feral and dangerous.',                           1, true),
(60, 9, 20, 'Quest',  'To secure our borders, go slay 5 Ice Dragons on the mountain.',                                                  2, true),
-- Q21
(61, 10, 21, 'None',  'Halt! This is the forbidden zone. None may enter.',                                                              1, true),
(62, 10, 21, 'None',  'Wait... you have the aura of one who has fought the Ice Dragons.',                                               2, true),
(63, 10, 21, 'Quest', 'Since you made it this far, help me explore this dangerous area.',                                               3, true),
-- Q22
(64, 10, 22, 'None',  'We have uncovered the origin of the codex... The truth is terrifying.',                                          1, true),
(65, 10, 22, 'None',  'A massive ancient golem guards the final piece of the puzzle.',                                                  2, true),
(66, 10, 22, 'Quest', 'Defeat the giant GolemBoss to claim the Golem Seal Book! Do not fail us.',                                       3, true),
-- Q23
(67, 11, 23, 'None',  'Stay back! The undead are relentless today.',                                                                    1, true),
(68, 11, 23, 'None',  'An ancient power is leaking, causing skeletons to multiply out of control.',                                     2, true),
(69, 11, 23, 'Quest', 'I can''t hold them off alone. Defeat 12 of them in the valley!',                                                 3, true),
-- Q24
(70, 11, 24, 'Quest', 'The animals are fleeing from the abandoned village Tide-Knell. Investigate it and find Natalie.',               1, true),
(71, 12, 24, 'None',  'Are you here to help me? I cannot leave this place...',                                                          2, true),
(72, 12, 24, 'Quest', 'Please... dig up what is buried under the small tree near the old well.',                                        3, true),
-- Q25
(73, 12, 25, 'None',  'Thank you for finding my remains. Now I can finally rest in peace.',                                             1, true),
(74, 12, 25, 'None',  'The ancient power leak was my doing. I am so sorry for the chaos.',                                              2, true),
(75, 12, 25, 'Reward','Take this key. It will unlock the doors to the island castle. Farewell.',                                        3, true),
-- Q26
(76, 13, 26, 'None',  'You actually survived the waves and made it to this deserted island.',                                           1, true),
(77, 13, 26, 'None',  'I need your assistance to prepare a ritual of return.',                                                          2, true),
(78, 13, 26, 'Quest', 'Help me collect 5 Ancient Leaves from the Northern Plateau.',                                                    3, true),
-- Q27
(79, 13, 27, 'None',  'We have everything we need. But a dark presence blocks our path.',                                               1, true),
(80, 13, 27, 'None',  'The UnderKing himself has awakened, and he guards the final Seal Book.',                                         2, true),
(81, 13, 27, 'Quest', 'You must end his reign! Defeat the UnderKing and claim the book!',                                               3, true),
-- Q28
(82, 13, 28, 'None',  'It is done. The UnderKing is defeated, and you have all 4 Seal Books.',                                          1, true),
(83, 13, 28, 'None',  'The fate of the Origin Tree now rests entirely in your hands.',                                                  2, true),
(84, 13, 28, 'Reward','Farewell, hero. I will use my power to open a portal back to the Elf Forest. Save the tree!',                   3, true),
-- Q29
(85, 2, 29, 'None',   'You have returned! And I can sense the power of the 4 Seal Books.',                                              1, true),
(86, 2, 29, 'None',   'The curse is breaking... The Origin Tree is finally healing!',                                                   2, true),
(87, 2, 29, 'Reward', 'Thank you! The Origin Tree is saved. But this is not the end... To be continued.',                               3, true)
ON CONFLICT DO NOTHING;
");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "EquipmentStats",
                keyColumn: "EquipmentStatsId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "NPCs",
                keyColumn: "NPCId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 33);
        }
    }
}
