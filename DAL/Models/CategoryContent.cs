using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the CategoryContent class.
    public class CategoryContent
    {
        // Executes category content id operation.
        public int CategoryContentId { get; set; }

        // Executes name operation.
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Executes slug operation.
        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        // Executes description operation.
        [MaxLength(500)]
        public string? Description { get; set; }

        // Executes icon url operation.
        public string? IconUrl { get; set; }

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Executes contents operation.
        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
