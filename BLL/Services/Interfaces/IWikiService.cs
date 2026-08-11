using BLL.DTOs;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Nghiệp vụ cho codex công khai (Visitor đọc web wiki).
    // Chỉ có View List và View Detail — không có Create/Update/Delete.
    //
    // Service này chuẩn hoá page/pageSize trước khi xuống repository, nên
    // controller không phải tự kiểm tra: một request công khai không thể xin
    // pageSize = 1_000_000.
    public interface IWikiService
    {
        // Lấy chỉ số khởi điểm của toàn bộ class.
        Task<IEnumerable<ClassConfigResponseDto>> GetClasses();

        // Lấy danh sách quái vật cho codex, có phân trang.
        Task<PagedResultDto<MonsterResponseDto>> GetMonsters(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder);

        // Lấy chi tiết quái vật cho codex (null nếu không tồn tại hoặc đã tắt).
        Task<MonsterDetailResponseDto?> GetMonsterById(int id);

        // Lấy danh sách vật phẩm cho codex, có phân trang.
        Task<PagedResultDto<ItemResponseDto>> GetItems(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder);

        // Lấy chi tiết vật phẩm cho codex (null nếu không tồn tại hoặc đã tắt).
        Task<ItemResponseDto?> GetItemById(int id);

        // Lấy danh sách kỹ năng cho codex, có phân trang.
        Task<PagedResultDto<SkillResponseDto>> GetSkills(
            int page, int pageSize, string? search, string? type);

        // Lấy chi tiết kỹ năng cho codex (null nếu không tồn tại hoặc đã tắt).
        Task<SkillResponseDto?> GetSkillById(int id);
    }
}
