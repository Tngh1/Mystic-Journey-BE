using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class PlayerProfileResponseDto
    {
        public int Id { get; set; }
        public Guid AccountId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public decimal Gold { get; set; }
        public decimal Gems { get; set; }
        public int Energy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePlayerProfileRequestDto
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public Guid AccountId { get; set; }

        [Required(ErrorMessage = "DisplayName is required.")]
        [StringLength(100, ErrorMessage = "DisplayName must not exceed 100 characters.")]
        public string DisplayName { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Class is required.")]
        public string Class { get; set; } = "Knight"; // Knight, Archer, Mage
    }

    public class UpdatePlayerProfileRequestDto
    {
        public string? AvatarUrl { get; set; }
        public string? Class { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Level must be at least 1.")]
        public int Level { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "ExperiencePoints cannot be negative.")]
        public int ExperiencePoints { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Gold cannot be negative.")]
        public decimal Gold { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Gems cannot be negative.")]
        public decimal Gems { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Energy cannot be negative.")]
        public int Energy { get; set; }
    }

    public class PlayerProfileApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
    }
}
