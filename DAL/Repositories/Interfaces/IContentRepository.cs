using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý nội dung game (bài viết, danh mục, block).
    // Game APIs: Xem nội dung, xem danh mục.
    // Admin APIs: Tạo, cập nhật, xóa nội dung và danh mục.
    public interface IContentRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy nội dung theo mã định danh.
        Task<Content?> GetContentById(int id);

        // Lấy nội dung kèm các block nội dung.
        Task<Content?> GetContentByIdWithBlocks(int id);

        // Lấy nội dung theo slug (đường dẫn tĩnh).
        Task<Content?> GetContentBySlug(string slug);

        // Lấy tất cả danh mục, có thể lọc theo tìm kiếm và trạng thái.
        Task<List<CategoryContent>> GetAllCategories(string? search, bool? isActive);

        // Lấy danh sách danh mục có phân trang, lọc theo tìm kiếm và trạng thái.
        Task<(int TotalCount, List<CategoryContent> Items)> GetCategoriesPaged(int page, int pageSize, string? search, bool? isActive);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật nội dung.
        Task<Content> UpdateContent(Content content);

        // Tạo nội dung mới kèm danh sách block.
        Task<Content> CreateContentWithBlocksAsync(Content content, IList<BlockContent> blocks);

        // Lấy danh mục theo mã định danh.
        Task<CategoryContent?> GetCategoryById(int id);

        // Tạo danh mục mới.
        Task<CategoryContent> CreateCategory(CategoryContent category);

        // Cập nhật danh mục.
        Task<CategoryContent> UpdateCategory(CategoryContent category);

        // Lấy block nội dung theo mã.
        Task<BlockContent?> GetBlockById(int id);

        // Tạo block nội dung mới.
        Task<BlockContent> CreateBlock(BlockContent block);

        // Cập nhật block nội dung.
        Task<BlockContent> UpdateBlock(BlockContent block);

        // Xóa block nội dung.
        Task RemoveBlock(int id);

        // Hủy xuất bản tất cả nội dung thuộc một danh mục.
        Task<int> UnpublishByCategoryIdAsync(int categoryId);

        // Lấy danh sách nội dung có phân trang, lọc theo tìm kiếm, trạng thái xuất bản và danh mục.
        Task<(int TotalCount, List<Content> Items)> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished, int? categoryId = null);
    }
}
