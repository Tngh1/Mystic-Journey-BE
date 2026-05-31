using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class CategoryContent
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? IconUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
