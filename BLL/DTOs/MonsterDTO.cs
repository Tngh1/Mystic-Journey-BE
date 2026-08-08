using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Monster ============
    public class MonsterResponseDto
    {
        public int MonsterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int MoveSpeed { get; set; }
        public int AttackSpeed { get; set; }
        public int CritRate { get; set; }
        public int CritDamage { get; set; }
        public int ExperienceReward { get; set; }
        public decimal GoldReward { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class MonsterDropResponseDto
    {
        public int MonsterDropId { get; set; }
        public int MonsterId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public double DropRate { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public bool IsGuaranteed { get; set; }
        public bool IsActive { get; set; }
    }

    public class MonsterDetailResponseDto : MonsterResponseDto
    {
        public List<MonsterDropResponseDto> MonsterDrops { get; set; } = new();
    }

    public class UpdateMonsterRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = "Normal";
        public string Description { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Level must be between 1 and 100.")]
        public int Level { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "MaxHp must be at least 1.")]
        public int MaxHp { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Atk cannot be negative.")]
        public int Atk { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Def cannot be negative.")]
        public int Def { get; set; }

        public int MoveSpeed { get; set; } = 100;
        public int AttackSpeed { get; set; } = 100;
        public int CritRate { get; set; } = 5;
        public int CritDamage { get; set; } = 150;
        public int ExperienceReward { get; set; } = 10;
        public decimal GoldReward { get; set; } = 5;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateMonsterDropRequestDto
    {
        [Required]
        public int ItemId { get; set; }

        [Range(0.0, 100.0, ErrorMessage = "DropRate must be between 0 and 100.")]
        public double DropRate { get; set; }

        public int MinQuantity { get; set; } = 1;
        public int MaxQuantity { get; set; } = 1;
        public bool IsGuaranteed { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ============ Spawn ============
    public class MonsterSpawnResponseDto
    {
        public int MonsterSpawnId { get; set; }
        public int MonsterId { get; set; }
        public string MonsterName { get; set; } = string.Empty;
        public string MonsterType { get; set; } = string.Empty;
        public string MapName { get; set; } = string.Empty;
        public string? RegionName { get; set; }
        public string? Location { get; set; }
        public int SpawnCount { get; set; }
        public int RespawnSeconds { get; set; }
        public int? DungeonId { get; set; }
        public string? DungeonName { get; set; }
        public bool IsDungeonRepeatable { get; set; }
        public bool IsActive { get; set; }
        public MonsterResponseDto Monster { get; set; } = new();
    }

    public class CreateMonsterSpawnRequestDto
    {
        [Required]
        public int MonsterId { get; set; }

        [Required, StringLength(100)]
        public string MapName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? RegionName { get; set; }

        public string? Location { get; set; }

        [Range(1, 50)]
        public int SpawnCount { get; set; } = 1;

        [Range(0, 86400)]
        public int RespawnSeconds { get; set; } = 60;

        public int? DungeonId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateMonsterSpawnRequestDto
    {
        [Range(1, 50)]
        public int SpawnCount { get; set; } = 1;

        [Range(0, 86400)]
        public int RespawnSeconds { get; set; } = 60;
    }

    // ============ Player catalog / bestiary ============
    public class PlayerMonsterCatalogItemDto
    {
        public int MonsterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int ExperienceReward { get; set; }
        public decimal GoldReward { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDiscovered { get; set; }
        public int TimesDefeated { get; set; }
    }

    // ============ Defeat / rewards ============
    public class MonsterDefeatRequestDto
    {
        public int? MonsterSpawnId { get; set; }
        public int? DungeonSessionId { get; set; }
    }

    public class MonsterDroppedItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemIconUrl { get; set; }
        public int Quantity { get; set; }
    }

    public class MonsterDefeatResponseDto
    {
        public int MonsterId { get; set; }
        public string MonsterName { get; set; } = string.Empty;
        public bool WasDiscovered { get; set; }
        public int ExperienceEarned { get; set; }
        public decimal GoldEarned { get; set; }
        public int PlayerLevel { get; set; }
        public int PlayerExperience { get; set; }
        public decimal PlayerGold { get; set; }
        public List<MonsterDroppedItemDto> DroppedItems { get; set; } = new();
    }
}
