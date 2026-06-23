using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class BlockContent
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Content reference
        public int ContentId { get; set; }
        public Content? Content { get; set; }

        // Block type: Text, Image, Video
        public string BlockType { get; set; } = "Text";

        public string? ContentData { get; set; }

        public string? MediaUrl { get; set; }

        public string? Caption { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
