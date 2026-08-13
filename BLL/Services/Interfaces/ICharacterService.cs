using BLL.DTOs;
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

        // Max HP thực tế = (chỉ số gốc + trang bị) × (1 + % danh hiệu) — đúng con số hiển thị
        // trên thanh máu. Mọi chỗ clamp CurrentHp phải dùng hàm này thay cho PlayerStat.MaxHp.
        Task<int> GetEffectiveMaxHp(int playerProfileId);

        // Đồng bộ danh sách Buff/Debuff của người chơi
        Task SyncBuffs(int playerProfileId, UpdatePlayerBuffsRequest request);

        // Nâng cấp thuộc tính nhân vật bằng Skill Points.
        // Skill Points được cấp tự động khi lên level (3 điểm mỗi level).
        Task<UpgradeAttributeResponseDto> UpgradeAttribute(int playerProfileId, UpgradeAttributeRequestDto request);

        // Lấy danh sách 5 lựa chọn nâng cấp chỉ số ngẫu nhiên (dùng AvailableStatPoints)
        Task<List<string>> GetLevelUpOptions(int playerProfileId);

        // Xác nhận nâng cấp 1 chỉ số từ danh sách 5 lựa chọn ngẫu nhiên
        Task<PlayerStatsResponseDto> AllocateStat(int playerProfileId, string statName);
    }
}
