using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class CategoryContent
    {
        public int CategoryContentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public string? IconUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
