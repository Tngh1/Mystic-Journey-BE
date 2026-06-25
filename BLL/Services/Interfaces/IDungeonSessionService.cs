using BLL.DTOs;

namespace BLL.Services.Interfaces
{
    public interface IDungeonSessionService
    {
        /// <summary>
        /// BR-01..05: Validates player + dungeon exist, checks energy (does NOT consume it),
        /// creates a DungeonSession with Status="Active", and seeds an empty DungeonProgress.
        /// </summary>
        Task<EnterDungeonResponseDto> EnterDungeon(int playerProfileId, int dungeonConfigId, List<string>? partyMembers = null);

        /// <summary>
        /// BR-06..07: Updates MonstersKilled, BossKilled, CompletionPercentage for an active session.
        /// </summary>
        Task<DungeonProgressResponseDto> UpdateProgress(int sessionId, int playerProfileId, UpdateDungeonProgressRequestDto request);

        /// <summary>
        /// BR-08..09: Validates boss is defeated, marks session Completed,
        /// returns chest preview. Does NOT grant rewards.
        /// </summary>
        Task<CompleteDungeonResponseDto> CompleteSession(int sessionId, int playerProfileId);

        /// <summary>
        /// BR-10: Validates session is completed + unclaimed, re-validates energy,
        /// then in a single transaction: consumes energy, rolls all rewards,
        /// upserts inventory, adds gold/XP, and marks session RewardClaimed.
        /// </summary>
        Task<ClaimDungeonRewardResponseDto> ClaimReward(int sessionId, int playerProfileId);
    }
}
