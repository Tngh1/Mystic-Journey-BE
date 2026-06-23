using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class SubCategoryContent
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? IconUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Parent category
        public int CategoryContentId { get; set; }
        public CategoryContent? CategoryContent { get; set; }

        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
