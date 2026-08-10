using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Content ============
    public class ContentResponseDto
    {
        public int ContentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

    public class ContentDetailResponseDto : ContentResponseDto
    {
        public List<BlockContentResponseDto> Blocks { get; set; } = new();
    }
    public class CreateContentWithBlocksRequestDto
    {
        [Required]
        [StringLength(250)]
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? CategoryId { get; set; }
        public bool IsPublished { get; set; } = false;

        public List<CreateContentBlockItemDto> Blocks { get; set; } = new();
    }

    public class CreateContentBlockItemDto
    {
        public string? ContentData { get; set; }
        public string? MediaUrl { get; set; }
        public string? Caption { get; set; }
        public string BlockType { get; set; } = "Text";
        public int? SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateContentRequestDto
    {
        [Required]
        [StringLength(250)]
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? CategoryId { get; set; }
        public bool IsPublished { get; set; }
    }

    // ============ CategoryContent ============
    public class CategoryContentResponseDto
    {
        public int CategoryContentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCategoryContentRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ============ BlockContent ============
    public class BlockContentResponseDto
    {
        public int BlockContentId { get; set; }
        public int ContentId { get; set; }
        public string? ContentData { get; set; }
        public string? MediaUrl { get; set; }
        public string? Caption { get; set; }
        public string BlockType { get; set; } = "Text";
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateBlockContentRequestDto
    {
        [Required]
        public int ContentId { get; set; }

        public string? ContentData { get; set; }
        public string? MediaUrl { get; set; }
        public string? Caption { get; set; }
        public string BlockType { get; set; } = "Text";
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateBlockContentRequestDto
    {
        public string? ContentData { get; set; }
        public string? MediaUrl { get; set; }
        public string? Caption { get; set; }
        public string BlockType { get; set; } = "Text";
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
