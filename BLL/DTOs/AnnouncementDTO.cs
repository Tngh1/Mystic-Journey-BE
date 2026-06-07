using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ GameAnnouncement ============
    public class GameAnnouncementResponseDto
    {
        public int GameAnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public bool IsActive { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateGameAnnouncementRequestDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; } = string.Empty;

        public string Type { get; set; } = "Info";
        public bool IsActive { get; set; } = true;
        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }

    public class UpdateGameAnnouncementRequestDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Type { get; set; } = "Info";
        public bool? IsActive { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }

    // ============ PlayerAnnouncement ============
    public class PlayerAnnouncementResponseDto
    {
        public int PlayerAnnouncementId { get; set; }
        public int PlayerProfileId { get; set; }
        public int GameAnnouncementId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
    }
}
