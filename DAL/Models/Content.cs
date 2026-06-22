using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Content
    {
        public int ContentId { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string? ThumbnailUrl { get; set; }

        public int? CategoryContentId { get; set; }
        public CategoryContent? CategoryContent { get; set; }

        public int? SubCategoryContentId { get; set; }
        public SubCategoryContent? SubCategoryContent { get; set; }

        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public Guid CreatedByAccountId { get; set; }
        public Account? CreatedByAccount { get; set; }

        public ICollection<BlockContent> BlockContents { get; set; } = new List<BlockContent>();
    }
}
