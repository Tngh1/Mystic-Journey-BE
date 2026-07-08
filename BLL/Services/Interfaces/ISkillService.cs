using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý skills (kỹ năng) trong game.
    // Game APIs: Xem danh sách skills, xem chi tiết skill.
    // Admin APIs: Tạo, cập nhật skill.
    public interface ISkillService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy danh sách skills của player đang đăng nhập.
        Task<PlayerMeSkillsResponseDto> GetMeSkills(int playerProfileId);

        // Nâng cấp skill của player.
        Task<PlayerSkillResponseDto> UpgradePlayerSkill(int actorPlayerProfileId, UpgradePlayerSkillRequestDto request);

        // Trang bị skill vào slot.
        Task<PlayerSkillResponseDto> EquipPlayerSkill(int actorPlayerProfileId, EquipSkillRequestDto request);

        // Mở khóa skill mới cho player.
        Task<PlayerSkillResponseDto> UnlockPlayerSkill(int actorPlayerProfileId, UnlockPlayerSkillRequestDto request);

        // Record that a skill was cast to track its cooldown
        Task<PlayerSkillResponseDto> RecordSkillCast(int actorPlayerProfileId, int playerSkillId);

        // Phá skill để lấy nguyên liệu.
        Task<PlayerSkillResponseDto?> DismantlePlayerSkill(int actorPlayerProfileId, DismantlePlayerSkillRequestDto request);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết skill theo ID.
        Task<SkillResponseDto?> GetSkillById(int id);

        // Lấy danh sách tất cả skills có phân trang và lọc.
        Task<PagedResultDto<SkillResponseDto>> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive);

        // Tạo skill mới.
        Task<SkillResponseDto> CreateSkill(CreateSkillRequestDto request);

        // Cập nhật skill hiện có.
        Task<SkillResponseDto> UpdateSkill(int id, UpdateSkillRequestDto request);
    }
}
