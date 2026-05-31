using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Content
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string? ThumbnailUrl { get; set; }

        // Category reference
        public int? CategoryContentId { get; set; }
        public CategoryContent? CategoryContent { get; set; }

        public bool IsPublished { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public ICollection<BlockContent> BlockContents { get; set; } = new List<BlockContent>();
    }
}
