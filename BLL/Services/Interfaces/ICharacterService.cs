using BLL.DTOs;
using System.Collections.Generic;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the ICharacterService class.
    public interface ICharacterService
    {

        Task<CharacterResponseDto> CreateCharacter(int playerProfileId, CreateCharacterRequestDto request);

        Task<PlayerStatsResponseDto> GetStats(int playerProfileId);

        Task UpdateHp(int playerProfileId, int currentHp);

        Task<int> GetEffectiveMaxHp(int playerProfileId);

        Task SyncBuffs(int playerProfileId, UpdatePlayerBuffsRequest request);

        Task<UpgradeAttributeResponseDto> UpgradeAttribute(int playerProfileId, UpgradeAttributeRequestDto request);

        Task<List<string>> GetLevelUpOptions(int playerProfileId);

        Task<PlayerStatsResponseDto> AllocateStat(int playerProfileId, string statName);
    }
}
