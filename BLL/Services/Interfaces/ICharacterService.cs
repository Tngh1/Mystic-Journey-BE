using BLL.DTOs;
using DAL.Models;
using System.Collections.Generic;

namespace BLL.Services.Interfaces
{
    // Quản lý nhân vật (character) của người chơi.
    // Cho phép tạo, xem chỉ số, cập nhật HP và nâng cấp thuộc tính.
    public interface ICharacterService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo nhân vật mới cho tài khoản.
        // Thiết lập display name và class cho player mới đăng ký.
        // Chỉ được gọi một lần cho mỗi tài khoản.
        Task<CharacterResponseDto> CreateCharacter(int playerProfileId, CreateCharacterRequestDto request);

        // Lấy danh sách chỉ số của nhân vật.
        // Bao gồm: CurrentHp, MaxHp, Atk, Def, MoveSpeed, AttackSpeed, CritRate, CritDamage,
        // DamageBonus, SkillPoints, TotalWins, TotalLosses, TotalKills, TotalDeaths.
        Task<PlayerStatsResponseDto> GetStats(int playerProfileId);

        // Cập nhật HP hiện tại của nhân vật (đồng bộ từ client).
        Task UpdateHp(int playerProfileId, int currentHp);

        // Đồng bộ danh sách Buff/Debuff của người chơi
        Task SyncBuffs(int playerProfileId, UpdatePlayerBuffsRequest request);

        // Nâng cấp thuộc tính nhân vật bằng Skill Points.
        // Skill Points được cấp tự động khi lên level (3 điểm mỗi level).
        Task<UpgradeAttributeResponseDto> UpgradeAttribute(int playerProfileId, UpgradeAttributeRequestDto request);

        // Lấy danh sách chỉ số khởi điểm của các Class (dành cho Web Wiki)
        Task<IEnumerable<ClassConfig>> GetAllClassConfigs();
    }
}
