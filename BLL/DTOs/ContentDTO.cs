using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the ContentResponseDto class.
    public class ContentResponseDto
    {
        // Executes content id operation.
        public int ContentId { get; set; }
        // Executes title operation.
        public string Title { get; set; } = string.Empty;
        // Executes slug operation.
        public string Slug { get; set; } = string.Empty;
        // Executes summary operation.
        public string? Summary { get; set; }
        // Executes thumbnail url operation.
        public string? ThumbnailUrl { get; set; }
        // Executes category id operation.
        public int? CategoryId { get; set; }
        // Executes category name operation.
        public string? CategoryName { get; set; }
        // Executes is published operation.
        public bool IsPublished { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
        // Executes published at operation.
        public DateTime? PublishedAt { get; set; }
    }

    // Initializes a new default instance of the ContentResponseDto class.
    public class ContentDetailResponseDto : ContentResponseDto
    {
        // Executes blocks operation.
        public List<BlockContentResponseDto> Blocks { get; set; } = new();
    }
    // Executes create content with blocks request dto operation.
    public class CreateContentWithBlocksRequestDto
    {
        // Executes title operation.
        [Required]
        [StringLength(250)]
        public string Title { get; set; } = string.Empty;

        // Executes summary operation.
        public string? Summary { get; set; }
        // Executes thumbnail url operation.
        public string? ThumbnailUrl { get; set; }
        // Executes category id operation.
        public int? CategoryId { get; set; }
        // Executes is published operation.
        public bool IsPublished { get; set; } = false;

        // Executes blocks operation.
        public List<CreateContentBlockItemDto> Blocks { get; set; } = new();
    }

    // Executes create content block item dto operation.
    public class CreateContentBlockItemDto
    {
        // Executes content data operation.
        public string? ContentData { get; set; }
        // Executes media url operation.
        public string? MediaUrl { get; set; }
        // Executes caption operation.
        public string? Caption { get; set; }
        // Executes block type operation.
        public string BlockType { get; set; } = "Text";
        // Executes sort order operation.
        public int? SortOrder { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update content request dto operation.
    public class UpdateContentRequestDto
    {
        // Executes title operation.
        [Required]
        [StringLength(250)]
        public string Title { get; set; } = string.Empty;

        // Executes summary operation.
        public string? Summary { get; set; }
        // Executes thumbnail url operation.
        public string? ThumbnailUrl { get; set; }
        // Executes category id operation.
        public int? CategoryId { get; set; }
        // Executes is published operation.
        public bool IsPublished { get; set; }
    }

    // Executes category content response dto operation.
    public class CategoryContentResponseDto
    {
        // Executes category content id operation.
        public int CategoryContentId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes slug operation.
        public string Slug { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
    }

    // Executes create category content request dto operation.
    public class CreateCategoryContentRequestDto
    {
        // Executes name operation.
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Executes slug operation.
        [StringLength(250)]
        public string Slug { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
        public string? Description { get; set; }
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes block content response dto operation.
    public class BlockContentResponseDto
    {
        // Executes block content id operation.
        public int BlockContentId { get; set; }
        // Executes content id operation.
        public int ContentId { get; set; }
        // Executes content data operation.
        public string? ContentData { get; set; }
        // Executes media url operation.
        public string? MediaUrl { get; set; }
        // Executes caption operation.
        public string? Caption { get; set; }
        // Executes block type operation.
        public string BlockType { get; set; } = "Text";
        // Executes sort order operation.
        public int SortOrder { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
    }

    // Executes create block content request dto operation.
    public class CreateBlockContentRequestDto
    {
        // Executes content id operation.
        [Required]
        public int ContentId { get; set; }

        // Executes content data operation.
        public string? ContentData { get; set; }
        // Executes media url operation.
        public string? MediaUrl { get; set; }
        // Executes caption operation.
        public string? Caption { get; set; }
        // Executes block type operation.
        public string BlockType { get; set; } = "Text";
        // Executes sort order operation.
        public int SortOrder { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update block content request dto operation.
    public class UpdateBlockContentRequestDto
    {
        // Executes content data operation.
        public string? ContentData { get; set; }
        // Executes media url operation.
        public string? MediaUrl { get; set; }
        // Executes caption operation.
        public string? Caption { get; set; }
        // Executes block type operation.
        public string BlockType { get; set; } = "Text";
        // Executes sort order operation.
        public int SortOrder { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }
}
