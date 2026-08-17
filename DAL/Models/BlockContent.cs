using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the BlockContent class.
    public class BlockContent
    {
        // Executes id operation.
        public int Id { get; set; }

        // Executes title operation.
        public string Title { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Executes content id operation.
        public int ContentId { get; set; }
        // Executes content operation.
        public Content? Content { get; set; }

        // Executes block type operation.
        public string BlockType { get; set; } = "Text";

        // Executes content data operation.
        public string? ContentData { get; set; }

        // Executes media url operation.
        public string? MediaUrl { get; set; }

        // Executes caption operation.
        public string? Caption { get; set; }

        // Executes sort order operation.
        public int SortOrder { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
    }
}
