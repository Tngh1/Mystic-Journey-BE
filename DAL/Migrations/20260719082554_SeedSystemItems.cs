using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <summary>
    /// Migration that inserts all system items (System Items).
    /// Run this migration BEFORE executing any seed endpoints.
    /// Uses INSERT ... ON CONFLICT DO NOTHING to be idempotent (safe to run multiple times).
    /// </summary>
    public partial class SeedSystemItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ─────────────────────────────────────────────────────────────────────────
            // ITEMS – Insert all system items (ON CONFLICT DO NOTHING = idempotent)
            // Order: Currency → Weapon → Armor → Consumable → Material → QuestItem
            // ─────────────────────────────────────────────────────────────────────────
            migrationBuilder.Sql(@"
INSERT INTO ""Items"" (""Name"", ""Description"", ""Type"", ""Rarity"", ""Slot"", ""BaseValue"", ""MaxStack"", ""IsActive"", ""CorruptionReduction"", ""CreatedAt"")
VALUES
    -- ── Currency ─────────────────────────────────────────────────────────────
    ('Gold',                 'In-game gold currency.',                                                                'Currency',    'Common',     'None',    1,    2147483647, true, 0, NOW()),
    ('Exp',                  'Experience points for leveling up.',                                                    'Currency',    'Common',     'None',    1,    2147483647, true, 0, NOW()),
    ('Gem',                  'Premium gem used to purchase high-tier items.',                                        'Currency',    'Rare',       'None',    5,    2147483647, true, 0, NOW()),
    ('Lucky Ticket',         'Lucky ticket used to spin the gacha banner.',                                          'Consumable',  'Rare',       'None',    1,    99,         true, 0, NOW()),

    -- ── Weapon ───────────────────────────────────────────────────────────────
    ('Iron Sword',           'Basic iron sword for beginner warriors.',                                              'Weapon',      'Common',     'Weapon',  150,  1,          true, 0, NOW()),
    ('Hunter Bow',           'A forest hunter bow, light and accurate.',                                             'Weapon',      'Common',     'Weapon',  150,  1,          true, 0, NOW()),
    ('Apprentice Staff',     'A novice magic staff for casting light spells.',                                       'Weapon',      'Common',     'Weapon',  150,  1,          true, 0, NOW()),
    ('Elven Blade',          'A glowing elven blade, forged deep in the ancient forest.',                            'Weapon',      'Epic',       'Weapon',  800,  1,          true, 0, NOW()),

    -- ── Armor ────────────────────────────────────────────────────────────────
    ('Leather Armor',        'Light leather armor that provides basic defense.',                                     'Armor',       'Common',     'Armor',   120,  1,          true, 0, NOW()),
    ('Iron Helmet',          'Sturdy iron helmet that protects the head from damage.',                               'Armor',       'Common',     'Helmet',  100,  1,          true, 0, NOW()),
    ('Wind Boots',           'Wind-infused boots that increase movement speed.',                                     'Armor',       'Uncommon',   'Boots',   200,  1,          true, 0, NOW()),
    ('Dragon Scale Armor',   'Legendary dragon scale armor offering supreme defense.',                               'Armor',       'Legendary',  'Armor',   2000, 1,          true, 0, NOW()),
    ('Phantom Cloak',        'Shadow cloak that boosts speed and evasion.',                                          'Armor',       'Epic',       'Armor',   900,  1,          true, 0, NOW()),
    ('Shadow Hood',          'Dark hood that increases critical strike damage.',                                     'Armor',       'Rare',       'Helmet',  500,  1,          true, 0, NOW()),
    ('Iron Gauntlets',       'Iron gauntlets that increase physical damage.',                                        'Armor',       'Common',     'Gloves',  120,  1,          true, 0, NOW()),
    ('Leather Gauntlets',    'Soft leather gauntlets that allow flexible combat.',                                   'Armor',       'Common',     'Gloves',  100,  1,          true, 0, NOW()),
    ('Copper Ring',          'Basic copper ring that slightly boosts stats.',                                        'Armor',       'Common',     'Ring',    80,   1,          true, 0, NOW()),
    ('Silver Necklace',      'Silver necklace that increases maximum energy.',                                       'Armor',       'Uncommon',   'Necklace',200,  1,          true, 0, NOW()),

    -- ── Consumable ───────────────────────────────────────────────────────────
    ('Small Health Potion',  'Small health potion that restores 80 HP.',                                             'Consumable',  'Common',     'None',    30,   99,         true, 0, NOW()),
    ('Large Health Potion',  'Large health potion that restores 200 HP.',                                            'Consumable',  'Uncommon',   'None',    80,   99,         true, 0, NOW()),
    ('Energy Elixir',        'Energy elixir that restores 60 Energy.',                                              'Consumable',  'Uncommon',   'None',    60,   99,         true, 0, NOW()),

    -- ── Material / Upgrade ───────────────────────────────────────────────────
    ('Skill Upgrade Stone',  'Magic stone used to upgrade player skills.',                                           'Material',    'Rare',       'None',    50,   999,        true, 0, NOW()),

    -- ── Quest Items ──────────────────────────────────────────────────────────────────
    ('White Flower',         'White flower collected in the fairy forest.',                                          'QuestItem',   'Common',     'None',    0,    99,         true, 0, NOW()),
    ('Wood Logs',            'Logs collected from the ancient forest.',                                              'QuestItem',   'Common',     'None',    0,    99,         true, 0, NOW()),
    ('Ancient Leaves',       'Ancient tree leaves collected from the fairy forest.',                                 'QuestItem',   'Common',     'None',    0,    99,         true, 0, NOW()),

    -- ── Seal Books (QuestItem – dropped by Bosses) ──────────────────────────────────
    ('Dragon Seal Book',     'Dragon Seal Book. Dropped by DragonBossIdle. Collect all 4 seal books to save the World Tree.', 'QuestItem', 'Epic', 'None', 0, 1, true, 0, NOW()),
    ('Golem Seal Book',      'Golem Seal Book. Dropped by GolemBoss.',                                              'QuestItem',   'Epic',       'None',    0,    1,          true, 0, NOW()),
    ('UnderKing Seal Book',  'UnderKing Seal Book. Dropped by the UnderKing boss.',                                 'QuestItem',   'Epic',       'None',    0,    1,          true, 0, NOW()),
    ('Swamp Seal Book',      'Swamp Demon Seal Book. Dropped by SwampDemon boss.',                                  'QuestItem',   'Epic',       'None',    0,    1,          true, 0, NOW())

ON CONFLICT DO NOTHING;
");

            // ─────────────────────────────────────────────────────────────────────
            // EQUIPMENT STATS – Insert stats for all equippable items
            // WHERE NOT EXISTS prevents duplicates if migration is re-run
            // ─────────────────────────────────────────────────────────────────────
            migrationBuilder.Sql(@"
INSERT INTO ""EquipmentStats"" (""ItemId"", ""BaseHp"", ""BaseAtk"", ""BaseDef"", ""BonusHp"", ""BonusAtk"", ""BonusDef"", ""BonusMoveSpeed"", ""BonusAttackSpeed"", ""BonusCritRate"", ""BonusCritDamage"", ""BonusDamageBonus"")
SELECT i.""ItemId"", s.base_hp, s.base_atk, s.base_def, s.bonus_hp, s.bonus_atk, s.bonus_def, s.move_spd, s.atk_spd, s.crit_rate, s.crit_dmg, s.dmg_bonus
FROM (VALUES
    --                 name                  hp    atk  def  bhp  batk bdef spd  aspd  cr   cd   db
    ('Iron Sword',         0,   35,  0,   0,   8,   0,   0,  0,  30,  50,  0),
    ('Hunter Bow',         0,   30,  0,   0,   6,   0,   0,  10, 40,  30,  0),
    ('Apprentice Staff',   0,   28,  0,   0,   5,   0,   0,  0,  20,  80,  10),
    ('Elven Blade',        0,   80,  0,   0,   20,  0,   0,  5,  60, 100,  15),
    ('Leather Armor',      50,  0,   12,  10,  0,   3,   0,  0,  0,   0,   0),
    ('Iron Helmet',        100, 0,   30,  20,  0,   8,   0,  0,  0,   0,   0),
    ('Wind Boots',         0,   0,   5,   0,   0,   2,   20, 0,  0,   0,   0),
    ('Dragon Scale Armor', 500, 0,   120, 100, 0,   30,  0,  0,  0,   0,   0),
    ('Phantom Cloak',      0,   0,   60,  0,   0,   15,  15, 0,  0,   0,   0),
    ('Shadow Hood',        0,   0,   20,  0,   0,   5,   0,  0,  80, 120,  0),
    ('Iron Gauntlets',     0,   20,  5,   0,   5,   2,   0,  0,  0,   0,   5),
    ('Leather Gauntlets',  0,   15,  3,   0,   3,   1,   5,  5,  0,   0,   0),
    ('Copper Ring',        30,  5,   3,   5,   2,   1,   0,  0,  10,  10,  0),
    ('Silver Necklace',    80,  0,   5,   20,  0,   2,   0,  0,  0,   0,   0)
) AS s(name, base_hp, base_atk, base_def, bonus_hp, bonus_atk, bonus_def, move_spd, atk_spd, crit_rate, crit_dmg, dmg_bonus)
JOIN ""Items"" i ON i.""Name"" = s.name
WHERE NOT EXISTS (
    SELECT 1 FROM ""EquipmentStats"" e WHERE e.""ItemId"" = i.""ItemId""
);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete EquipmentStats first (FK constraint)
            migrationBuilder.Sql(@"
DELETE FROM ""EquipmentStats""
WHERE ""ItemId"" IN (
    SELECT ""ItemId"" FROM ""Items""
    WHERE ""Name"" IN (
        'Iron Sword','Hunter Bow','Apprentice Staff','Elven Blade',
        'Leather Armor','Iron Helmet','Wind Boots','Dragon Scale Armor',
        'Phantom Cloak','Shadow Hood','Iron Gauntlets','Leather Gauntlets',
        'Copper Ring','Silver Necklace'
    )
);
");
            // Delete all system items
            migrationBuilder.Sql(@"
DELETE FROM ""Items""
WHERE ""Name"" IN (
    'Gold','Exp','Gem','Lucky Ticket',
    'Iron Sword','Hunter Bow','Apprentice Staff','Elven Blade',
    'Leather Armor','Iron Helmet','Wind Boots','Dragon Scale Armor',
    'Phantom Cloak','Shadow Hood','Iron Gauntlets','Leather Gauntlets',
    'Copper Ring','Silver Necklace',
    'Small Health Potion','Large Health Potion','Energy Elixir',
    'Skill Upgrade Stone',
    'White Flower','Wood Logs','Ancient Leaves',
    'Dragon Seal Book','Golem Seal Book','UnderKing Seal Book','Swamp Seal Book'
);
");
        }
    }
}
