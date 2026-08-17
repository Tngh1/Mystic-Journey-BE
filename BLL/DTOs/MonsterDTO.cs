using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the MonsterResponseDto class.
    public class MonsterResponseDto
    {
        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Supported monster types: Normal, Elite, or Boss; the type controls presentation and encounter behavior.
        public string Type { get; set; } = string.Empty;
        // Executes description operation.
        public string Description { get; set; } = string.Empty;
        // Executes level operation.
        public int Level { get; set; }
        // Executes max hp operation.
        public int MaxHp { get; set; }
        // Executes atk operation.
        public int Atk { get; set; }
        // Executes def operation.
        public int Def { get; set; }
        // Executes move speed operation.
        public int MoveSpeed { get; set; }
        // Executes attack speed operation.
        public int AttackSpeed { get; set; }
        // Executes crit rate operation.
        public int CritRate { get; set; }
        // Executes crit damage operation.
        public int CritDamage { get; set; }
        // Executes experience reward operation.
        public int ExperienceReward { get; set; }
        // Executes gold reward operation.
        public decimal GoldReward { get; set; }
        // Executes image url operation.
        public string? ImageUrl { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
    }

    // Executes monster drop response dto operation.
    public class MonsterDropResponseDto
    {
        // Executes monster drop id operation.
        public int MonsterDropId { get; set; }
        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
        // Executes drop rate operation.
        public double DropRate { get; set; }
        // Executes min quantity operation.
        public int MinQuantity { get; set; }
        // Executes max quantity operation.
        public int MaxQuantity { get; set; }
        // Executes is guaranteed operation.
        public bool IsGuaranteed { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
    }

    // Initializes a new default instance of the MonsterResponseDto class.
    public class MonsterDetailResponseDto : MonsterResponseDto
    {
        // Executes monster drops operation.
        public List<MonsterDropResponseDto> MonsterDrops { get; set; } = new();
    }

    // Executes update monster request dto operation.
    public class UpdateMonsterRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        // Supported monster types: Normal, Elite, or Boss; the type controls presentation and encounter behavior.
        public string Type { get; set; } = "Normal";
        // Executes description operation.
        public string Description { get; set; } = string.Empty;

        // Executes level operation.
        [Range(1, 100, ErrorMessage = "Level must be between 1 and 100.")]
        public int Level { get; set; } = 1;

        // Executes max hp operation.
        [Range(1, int.MaxValue, ErrorMessage = "MaxHp must be at least 1.")]
        public int MaxHp { get; set; }

        // Executes atk operation.
        [Range(0, int.MaxValue, ErrorMessage = "Atk cannot be negative.")]
        public int Atk { get; set; }

        // Executes def operation.
        [Range(0, int.MaxValue, ErrorMessage = "Def cannot be negative.")]
        public int Def { get; set; }

        // Executes move speed operation.
        public int MoveSpeed { get; set; } = 100;
        // Executes attack speed operation.
        public int AttackSpeed { get; set; } = 100;
        // Executes crit rate operation.
        public int CritRate { get; set; } = 5;
        // Executes crit damage operation.
        public int CritDamage { get; set; } = 150;
        // Executes experience reward operation.
        public int ExperienceReward { get; set; } = 10;
        // Executes gold reward operation.
        public decimal GoldReward { get; set; } = 5;
        // Executes image url operation.
        public string? ImageUrl { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes create monster drop request dto operation.
    public class CreateMonsterDropRequestDto
    {
        // Executes item id operation.
        [Required]
        public int ItemId { get; set; }

        // Executes drop rate operation.
        [Range(0.0, 100.0, ErrorMessage = "DropRate must be between 0 and 100.")]
        public double DropRate { get; set; }

        // Executes min quantity operation.
        public int MinQuantity { get; set; } = 1;
        // Executes max quantity operation.
        public int MaxQuantity { get; set; } = 1;
        // Executes is guaranteed operation.
        public bool IsGuaranteed { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes monster spawn response dto operation.
    public class MonsterSpawnResponseDto
    {
        // Executes monster spawn id operation.
        public int MonsterSpawnId { get; set; }
        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes monster name operation.
        public string MonsterName { get; set; } = string.Empty;
        // Supported monster types: Normal, Elite, or Boss; the type controls presentation and encounter behavior.
        public string MonsterType { get; set; } = string.Empty;
        // Executes map name operation.
        public string MapName { get; set; } = string.Empty;
        // Executes region name operation.
        public string? RegionName { get; set; }
        // Executes location operation.
        public string? Location { get; set; }
        // Executes spawn count operation.
        public int SpawnCount { get; set; }
        // Executes respawn seconds operation.
        public int RespawnSeconds { get; set; }
        // Executes dungeon id operation.
        public int? DungeonId { get; set; }
        // Executes dungeon name operation.
        public string? DungeonName { get; set; }
        // Executes is dungeon repeatable operation.
        public bool IsDungeonRepeatable { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes monster operation.
        public MonsterResponseDto Monster { get; set; } = new();
    }

    // Executes create monster spawn request dto operation.
    public class CreateMonsterSpawnRequestDto
    {
        // Executes monster id operation.
        [Required]
        public int MonsterId { get; set; }

        // Executes map name operation.
        [Required, StringLength(100)]
        public string MapName { get; set; } = string.Empty;

        // Executes region name operation.
        [StringLength(100)]
        public string? RegionName { get; set; }

        // Executes location operation.
        public string? Location { get; set; }

        // Executes spawn count operation.
        [Range(1, 50)]
        public int SpawnCount { get; set; } = 1;

        // Executes respawn seconds operation.
        [Range(0, 86400)]
        public int RespawnSeconds { get; set; } = 60;

        // Executes dungeon id operation.
        public int? DungeonId { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update monster spawn request dto operation.
    public class UpdateMonsterSpawnRequestDto
    {
        // Executes spawn count operation.
        [Range(1, 50)]
        public int SpawnCount { get; set; } = 1;

        // Executes respawn seconds operation.
        [Range(0, 86400)]
        public int RespawnSeconds { get; set; } = 60;
    }

    // Executes player monster catalog item dto operation.
    public class PlayerMonsterCatalogItemDto
    {
        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Supported monster types: Normal, Elite, or Boss; the type controls presentation and encounter behavior.
        public string Type { get; set; } = string.Empty;
        // Executes description operation.
        public string Description { get; set; } = string.Empty;
        // Executes level operation.
        public int Level { get; set; }
        // Executes max hp operation.
        public int MaxHp { get; set; }
        // Executes atk operation.
        public int Atk { get; set; }
        // Executes def operation.
        public int Def { get; set; }
        // Executes experience reward operation.
        public int ExperienceReward { get; set; }
        // Executes gold reward operation.
        public decimal GoldReward { get; set; }
        // Executes image url operation.
        public string? ImageUrl { get; set; }
        // Executes is discovered operation.
        public bool IsDiscovered { get; set; }
        // Executes times defeated operation.
        public int TimesDefeated { get; set; }
    }

    // Executes monster defeat request dto operation.
    public class MonsterDefeatRequestDto
    {
        // Executes monster spawn id operation.
        public int? MonsterSpawnId { get; set; }
        // Executes dungeon session id operation.
        public int? DungeonSessionId { get; set; }
    }

    // Executes monster dropped item dto operation.
    public class MonsterDroppedItemDto
    {
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string ItemName { get; set; } = string.Empty;
        // Executes item icon url operation.
        public string? ItemIconUrl { get; set; }
        // Executes quantity operation.
        public int Quantity { get; set; }
    }

    // Executes monster defeat response dto operation.
    public class MonsterDefeatResponseDto
    {
        // Executes monster id operation.
        public int MonsterId { get; set; }
        // Executes monster name operation.
        public string MonsterName { get; set; } = string.Empty;
        // Executes was discovered operation.
        public bool WasDiscovered { get; set; }
        // Executes experience earned operation.
        public int ExperienceEarned { get; set; }
        // Executes gold earned operation.
        public decimal GoldEarned { get; set; }
        // Executes player level operation.
        public int PlayerLevel { get; set; }
        // Executes player experience operation.
        public int PlayerExperience { get; set; }
        // Executes player gold operation.
        public decimal PlayerGold { get; set; }
        // Executes dropped items operation.
        public List<MonsterDroppedItemDto> DroppedItems { get; set; } = new();
    }
}
