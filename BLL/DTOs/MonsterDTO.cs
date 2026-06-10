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

    public class CreateMonsterRequestDto
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
}
