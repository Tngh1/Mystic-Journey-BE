using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class BlockContent
    {
        public int BlockContentId { get; set; }

        public int ContentId { get; set; }
        public Content? Content { get; set; }

        public string? ContentData { get; set; }

        public string? MediaUrl { get; set; }

        public string? Caption { get; set; }

        // Types: Text, Image, Video
        public string BlockType { get; set; } = "Text";

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
