using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý skills (kỹ năng) trong game.
    // Game APIs: Xem danh sách skills, xem chi tiết skill, quản lý skills của người chơi.
    // Admin APIs: Tạo, cập nhật skill.
    public interface ISkillRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy kỹ năng của người chơi theo mã.
        Task<PlayerSkill?> GetPlayerSkillById(int playerSkillId);

        // Lấy tất cả kỹ năng của một người chơi.
        Task<List<PlayerSkill>> GetPlayerSkillsByPlayerId(int playerProfileId);

        // Tìm kỹ năng theo danh sách tên.
        Task<List<Skill>> GetSkillsByNames(string[] names);

        // Tìm kỹ năng theo tên chính xác.
        Task<Skill?> GetSkillByName(string name);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy kỹ năng theo mã định danh.
        Task<Skill?> GetSkillById(int id);

        // Lấy tất cả kỹ năng trong hệ thống.
        Task<List<Skill>> GetAllSkillsAsync();

        // Lấy danh sách kỹ năng có phân trang, lọc theo tìm kiếm, loại và trạng thái.
        Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive);

        // Tạo kỹ năng mới trong hệ thống.
        Task<Skill> CreateSkill(Skill skill);

        // Cập nhật thông tin kỹ năng.
        Task<Skill> UpdateSkill(Skill skill);

        // Thêm kỹ năng mới cho người chơi.
        Task<PlayerSkill> CreatePlayerSkill(PlayerSkill playerSkill);

        // Cập nhật kỹ năng của người chơi.
        Task<PlayerSkill> UpdatePlayerSkill(PlayerSkill playerSkill);

        // Xóa kỹ năng của người chơi.
        Task DeletePlayerSkill(PlayerSkill playerSkill);
    }
}
