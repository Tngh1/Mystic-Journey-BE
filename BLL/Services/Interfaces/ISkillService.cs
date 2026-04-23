using BLL.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ISkillService
    {
        Task<SkillListResponseDto> GetAllSkillsAsync();
        Task<SkillListResponseDto> GetSkillsByClassAsync(PlayerProfile.CharacterClass characterClass);
        Task<SkillListResponseDto> GetAvailableSkillsAsync(Guid accountId);
        Task<SkillApiResponseDto> GetSkillByIdAsync(Guid skillId);
        Task<SkillListResponseDto> GetPlayerSkillsAsync(Guid accountId);
        Task<SkillListResponseDto> GetEquippedSkillsAsync(Guid accountId);
        Task<SkillApiResponseDto> UnlockSkillAsync(Guid accountId, UnlockSkillRequestDto request);
        Task<SkillApiResponseDto> UpgradeSkillAsync(Guid accountId, UpgradeSkillRequestDto request);
        Task<SkillApiResponseDto> EquipSkillAsync(Guid accountId, EquipSkillRequestDto request);
        Task<SkillApiResponseDto> UnequipSkillAsync(Guid accountId, Guid playerSkillId);
    }
}
