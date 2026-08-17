using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Content class.
    public class Content
    {
        // Executes content id operation.
        public int ContentId { get; set; }

        // Executes title operation.
        [Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        // Executes slug operation.
        [Required, MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        // Executes summary operation.
        public string? Summary { get; set; }

        // Executes thumbnail url operation.
        public string? ThumbnailUrl { get; set; }

        // Executes category content id operation.
        public int? CategoryContentId { get; set; }
        // Executes category content operation.
        public CategoryContent? CategoryContent { get; set; }

        // Executes is published operation.
        public bool IsPublished { get; set; } = false;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
        // Executes published at operation.
        public DateTime? PublishedAt { get; set; }

        // Executes block contents operation.
        public ICollection<BlockContent> BlockContents { get; set; } = new List<BlockContent>();
    }
}
