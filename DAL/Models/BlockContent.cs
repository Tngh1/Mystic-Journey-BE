using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class BlockContent
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        // Content reference
        public int ContentId { get; set; }
        public Content? Content { get; set; }
        public string? ContentData { get; set; } = string.Empty;
        public string? MediaUrl { get; set; } = string.Empty;
        public string? Caption { get; set; } = string.Empty;
        // Types: Text, Image, Video
        public string BlockType { get; set; } = "Text";
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
