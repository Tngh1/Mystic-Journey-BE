using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class GameSetting
    {
        public int GameSettingId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Value { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Guid? CreatedByAccountId { get; set; }
        public Account? CreatedByAccount { get; set; }

        public Guid? UpdatedByAccountId { get; set; }
        public Account? UpdatedByAccount { get; set; }
    }
}
