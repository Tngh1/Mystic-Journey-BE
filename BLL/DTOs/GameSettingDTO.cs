using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ GameSetting ============
    public class GameSettingResponseDto
    {
        public int GameSettingId { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class UpdateGameSettingRequestDto
    {
        public string? Value { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
