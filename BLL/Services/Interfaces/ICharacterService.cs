using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface ICharacterService
    {
        /// <summary>
        /// Sets the character name and class for a newly registered player,
        /// and seeds their base PlayerStat row. Can only be called once per profile.
        /// </summary>
        Task<CharacterResponseDto> CreateCharacter(int playerProfileId, CreateCharacterRequestDto request);

        /// <summary>
        /// Returns all current player stats (HP, ATK, DEF, speeds, crits, Skill Points, etc.).
        /// </summary>
        Task<PlayerStatsResponseDto> GetStats(int playerProfileId);

        /// <summary>
        /// Spends Skill Points to increase a chosen attribute.
        /// Skill Points are granted on level-up (handled externally when XP thresholds are crossed).
        /// </summary>
        Task<UpgradeAttributeResponseDto> UpgradeAttribute(int playerProfileId, UpgradeAttributeRequestDto request);
    }
}
