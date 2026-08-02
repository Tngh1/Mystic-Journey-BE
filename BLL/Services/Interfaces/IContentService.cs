using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý contents (nội dung) và categories.
    // Game APIs: Xem contents, categories.
    // Admin APIs: Tạo, cập nhật, publish contents, quản lý categories và blocks.
    public interface IContentService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết content theo ID.
        Task<ContentDetailResponseDto?> GetContentById(int id);

        // Lấy content theo slug.
        Task<ContentDetailResponseDto?> GetContentBySlug(string slug);

        // Lấy danh sách tất cả contents có phân trang và lọc.
        Task<PagedResultDto<ContentResponseDto>> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished, int? categoryId = null);

        // Lấy danh sách categories.
        Task<List<CategoryContentResponseDto>> GetAllCategories(string? search = null, bool? isActive = null);

        // Lấy danh sách categories có phân trang.
        Task<PagedResultDto<CategoryContentResponseDto>> GetCategoriesPaged(int page, int pageSize, string? search = null, bool? isActive = null);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo content mới kèm blocks.
        Task<ContentDetailResponseDto> CreateContentWithBlocksAsync(CreateContentWithBlocksRequestDto request);

        // Cập nhật content hiện có.
        Task<ContentResponseDto> UpdateContent(int id, UpdateContentRequestDto request);

        // Publish content.
        Task<ContentResponseDto> PublishContent(int id);

        // Tạo category mới.
        Task<CategoryContentResponseDto> CreateCategory(CreateCategoryContentRequestDto request);

        // Cập nhật category hiện có.
        Task<CategoryContentResponseDto> UpdateCategory(int id, CreateCategoryContentRequestDto request);

        // Tạo block mới.
        Task<BlockContentResponseDto> CreateBlock(CreateBlockContentRequestDto request);

        // Cập nhật block hiện có.
        Task<BlockContentResponseDto> UpdateBlock(int id, UpdateBlockContentRequestDto request);

        // Xóa block.
        Task RemoveBlock(int id);
    }
}
