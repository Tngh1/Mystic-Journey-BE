using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Repository đọc dữ liệu codex công khai cho web wiki (Visitor).
    // Chỉ có 2 dạng truy vấn: View List (phân trang) và View Detail (theo id).
    //
    // Điểm cốt lõi: MỌI truy vấn ở đây đều ghim IsActive = true ngay tại tầng dữ
    // liệu, nên không có tham số isActive để truyền vào. Một endpoint công khai
    // không thể vô tình để lộ bản nháp, kể cả khi controller viết sai.
    //
    // Không có Create/Update/Delete: dashboard dùng ItemRepository /
    // MonsterRepository / SkillRepository như trước.
    public interface IWikiRepository
    {
        // ── Classes ────────────────────────────────────────────────
        // Lấy chỉ số khởi điểm của toàn bộ class (3 class, không phân trang).
        Task<List<ClassConfig>> GetClassConfigs();

        // ── Monsters ───────────────────────────────────────────────
        // Lấy danh sách quái vật đang hoạt động, có phân trang.
        Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(
            int page, int pageSize, string? search, string? type, string? sortBy, string? sortOrder);

        // Lấy chi tiết quái vật đang hoạt động kèm vật phẩm rơi.
        Task<Monster?> GetMonsterById(int id);

        // ── Items ──────────────────────────────────────────────────
        // Lấy danh sách vật phẩm đang hoạt động, có phân trang.
        Task<(int TotalCount, List<Item> Items)> GetItemsPaged(
            int page, int pageSize, string? search, string? type, string? rarity, string? sortBy, string? sortOrder);

        // Lấy chi tiết vật phẩm đang hoạt động kèm chỉ số trang bị.
        Task<Item?> GetItemById(int id);

        // ── Skills ─────────────────────────────────────────────────
        // Lấy danh sách kỹ năng đang hoạt động, có phân trang.
        Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(
            int page, int pageSize, string? search, string? type);

        // Lấy chi tiết kỹ năng đang hoạt động.
        Task<Skill?> GetSkillById(int id);
    }
}
