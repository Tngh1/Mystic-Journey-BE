using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class DungeonSessionService : IDungeonSessionService
    {
        private readonly MysticJourneyDbContext _context;
        private readonly IDungeonConfigRepository _dungeonConfigRepository;
        private readonly IDungeonSessionRepository _sessionRepository;
        private readonly IDungeonProgressRepository _progressRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IPlayerProfileService _playerProfileService;

        public DungeonSessionService(
            MysticJourneyDbContext context,
            IDungeonConfigRepository dungeonConfigRepository,
            IDungeonSessionRepository sessionRepository,
            IDungeonProgressRepository progressRepository,
            IPlayerProfileRepository profileRepository,
            IPlayerProfileService playerProfileService)
        {
            _context = context;
            _dungeonConfigRepository = dungeonConfigRepository;
            _sessionRepository = sessionRepository;
            _progressRepository = progressRepository;
            _profileRepository = profileRepository;
            _playerProfileService = playerProfileService;
        }

        // ── 1. Enter Dungeon ─────────────────────────────────────────────────────────

        public async Task<EnterDungeonResponseDto> EnterDungeon(int playerProfileId, int dungeonConfigId, List<string>? partyMembers = null)
        {
            // BR-01: Character must exist
            var profile = await _profileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile {playerProfileId} not found.");

            // Recalculate energy first
            _playerProfileService.RecalculateEnergy(profile);
            await _profileRepository.UpdatePlayerProfile(profile);

            // BR-02: Dungeon must exist and be active
            var dungeon = await _dungeonConfigRepository.GetByIdWithChest(dungeonConfigId)
                ?? throw new KeyNotFoundException($"Dungeon {dungeonConfigId} not found or is not active.");

            // BR-03: Energy must be sufficient (NOT consumed here — BR-04/05)
            if (profile.CurrentEnergy < dungeon.EnergyCost)
                throw new InvalidOperationException(
                    $"Insufficient energy. Required: {dungeon.EnergyCost}, Current: {profile.CurrentEnergy}.");

            // Validate party data - total party members (including host) must not exceed MaxMembers
            int totalMembersCount = 1 + (partyMembers?.Count ?? 0);
            if (totalMembersCount > dungeon.MaxMembers)
                throw new InvalidOperationException($"Party exceeds maximum allowed size of {dungeon.MaxMembers}.");

            // Prevent duplicate concurrent active sessions for the same dungeon
            var existing = await _sessionRepository.GetActiveSession(playerProfileId, dungeonConfigId);
            if (existing != null)
                throw new InvalidOperationException(
                    $"You already have an active session (#{existing.DungeonSessionId}) for this dungeon. Complete or abandon it first.");

            // Create session — Status = "Active", energy untouched
            var session = new DungeonSession
            {
                PlayerProfileId = playerProfileId,
                DungeonConfigId = dungeonConfigId,
                EnterTime = DateTime.UtcNow,
                Status = "Active",
                IsRewardClaimed = false,
                PartyMembers = partyMembers != null ? string.Join(",", partyMembers) : string.Empty
            };
            await _sessionRepository.Create(session);

            // Seed an empty DungeonProgress row for the session
            var progress = new DungeonProgress
            {
                DungeonSessionId = session.DungeonSessionId,
                MonstersKilled = 0,
                BossKilled = false,
                CompletionPercentage = 0
            };
            await _progressRepository.Create(progress);

            return new EnterDungeonResponseDto
            {
                DungeonSessionId = session.DungeonSessionId,
                PlayerProfileId = playerProfileId,
                DungeonConfigId = dungeonConfigId,
                DungeonName = dungeon.Name,
                EnergyCost = dungeon.EnergyCost,
                PlayerCurrentEnergy = profile.CurrentEnergy,
                EnterTime = session.EnterTime,
                Status = session.Status,
                PartyMembers = partyMembers ?? new List<string>()
            };
        }

        // ── 2. Update Progress ────────────────────────────────────────────────────────

        public async Task<DungeonProgressResponseDto> UpdateProgress(
            int sessionId, int playerProfileId, UpdateDungeonProgressRequestDto request)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            // Ownership check
            if (session.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You do not own this dungeon session.");

            // BR-07: Session must be Active
            if (session.Status != "Active")
                throw new InvalidOperationException(
                    $"Session {sessionId} is not active (current status: {session.Status}). Progress can only be updated for active sessions.");

            // Upsert progress
            var progress = session.Progress;
            if (progress == null)
            {
                progress = new DungeonProgress { DungeonSessionId = sessionId };
                progress.MonstersKilled = request.MonstersKilled;
                progress.BossKilled = request.BossKilled;
                progress.CompletionPercentage = request.CompletionPercentage;
                progress.ExtraData = request.ExtraData;
                await _progressRepository.Create(progress);
            }
            else
            {
                progress.MonstersKilled = request.MonstersKilled;
                progress.BossKilled = request.BossKilled;
                progress.CompletionPercentage = request.CompletionPercentage;
                progress.ExtraData = request.ExtraData;
                await _progressRepository.Update(progress);
            }

            return new DungeonProgressResponseDto
            {
                DungeonProgressId = progress.DungeonProgressId,
                DungeonSessionId = sessionId,
                MonstersKilled = progress.MonstersKilled,
                BossKilled = progress.BossKilled,
                CompletionPercentage = progress.CompletionPercentage,
                ExtraData = progress.ExtraData,
                UpdatedAt = progress.UpdatedAt,
                SessionStatus = session.Status
            };
        }

        // ── 3. Complete Session ───────────────────────────────────────────────────────

        public async Task<CompleteDungeonResponseDto> CompleteSession(int sessionId, int playerProfileId)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            // Ownership check
            if (session.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You do not own this dungeon session.");

            // Must be Active to transition to Completed
            if (session.Status != "Active")
                throw new InvalidOperationException(
                    $"Session {sessionId} cannot be completed (current status: {session.Status}).");

            // Boss must be defeated before completing
            var progress = session.Progress;
            if (progress == null || !progress.BossKilled)
                throw new InvalidOperationException(
                    "The dungeon boss has not been defeated yet. Defeat the boss to complete the dungeon.");

            // Mark as Completed — NO rewards granted (BR-09)
            session.Status = "Completed";
            session.CompletedTime = DateTime.UtcNow;
            await _sessionRepository.Update(session);

            // Build chest preview DTO (items visible to player before claiming)
            ChestResponseDto? chestDto = null;
            if (session.DungeonConfig?.Chest != null)
                chestDto = MapChestToDto(session.DungeonConfig.Chest);

            return new CompleteDungeonResponseDto
            {
                DungeonSessionId = sessionId,
                Status = session.Status,
                CompletedTime = session.CompletedTime!.Value,
                RewardChest = chestDto,
                Message = "Dungeon completed! Call claim-reward to collect your rewards."
            };
        }

        // ── 4. Claim Reward (TRANSACTIONAL) ──────────────────────────────────────────

        public async Task<ClaimDungeonRewardResponseDto> ClaimReward(int sessionId, int playerProfileId)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            // Ownership check
            if (session.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You do not own this dungeon session.");

            // BR-08: Session must be Completed
            if (session.Status != "Completed")
                throw new InvalidOperationException(
                    $"Session {sessionId} cannot have rewards claimed (status: {session.Status}). Complete the dungeon first.");

            // Guard against duplicate claims
            if (session.IsRewardClaimed)
                throw new InvalidOperationException("Rewards have already been claimed for this session.");

            // Load player profile (fresh, outside transaction to avoid stale reads)
            var profile = await _profileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile {playerProfileId} not found.");

            // Recalculate energy first
            _playerProfileService.RecalculateEnergy(profile);

            // BR-10: Re-validate energy before consuming
            var dungeon = session.DungeonConfig
                ?? throw new InvalidOperationException("Dungeon configuration is missing from session.");

            if (profile.CurrentEnergy < dungeon.EnergyCost)
                throw new InvalidOperationException(
                    $"Insufficient energy to claim reward. Required: {dungeon.EnergyCost}, Current: {profile.CurrentEnergy}.");

            // ── BEGIN TRANSACTION ────────────────────────────────────────────────────
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1 — Consume energy
                profile.CurrentEnergy -= dungeon.EnergyCost;
                profile.TotalDungeonClears += 1;

                // Step 2 — Roll gold reward
                var chest = dungeon.Chest;
                var goldEarned = 0;
                var experienceEarned = 0;
                var rewardItems = new List<DungeonRewardItemDto>();

                if (chest != null)
                {
                    goldEarned = chest.GoldMaxReward > chest.GoldMinReward
                        ? Random.Shared.Next(chest.GoldMinReward, chest.GoldMaxReward + 1)
                        : chest.GoldMinReward;

                    experienceEarned = chest.ExperienceReward;

                    // Step 3 — Roll each ChestItem by DropRate
                    foreach (var chestItem in chest.ChestItems)
                    {
                        var drops = chestItem.IsGuaranteed ||
                                    Random.Shared.NextDouble() * 100 <= (double)chestItem.DropRate;
                        if (!drops) continue;

                        var quantity = chestItem.QuantityMax > chestItem.QuantityMin
                            ? Random.Shared.Next(chestItem.QuantityMin, chestItem.QuantityMax + 1)
                            : chestItem.QuantityMin;

                        // Step 4 — Upsert inventory
                        await UpsertInventoryItem(playerProfileId, chestItem.ItemId, quantity);

                        if (chestItem.Item != null)
                        {
                            rewardItems.Add(new DungeonRewardItemDto
                            {
                                ItemId = chestItem.ItemId,
                                ItemName = chestItem.Item.Name,
                                ItemIconUrl = chestItem.Item.IconUrl,
                                ItemType = chestItem.Item.Type,
                                Rarity = chestItem.Item.Rarity,
                                Quantity = quantity
                            });
                        }
                    }
                }

                // Step 5 — Apply gold + XP to profile
                profile.Gold += goldEarned;
                profile.ExperiencePoints += experienceEarned;
                profile.UpdatedAt = DateTime.UtcNow;
                _context.PlayerProfiles.Update(profile);

                // Step 6 — Mark session as RewardClaimed
                session.IsRewardClaimed = true;
                session.Status = "RewardClaimed";
                session.UpdatedAt = DateTime.UtcNow;
                _context.DungeonSessions.Update(session);

                // Commit all changes atomically
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new ClaimDungeonRewardResponseDto
                {
                    DungeonSessionId = sessionId,
                    Success = true,
                    Message = "Rewards claimed successfully!",
                    EnergyConsumed = dungeon.EnergyCost,
                    GoldEarned = goldEarned,
                    ExperienceEarned = experienceEarned,
                    Items = rewardItems
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
            // ── END TRANSACTION ──────────────────────────────────────────────────────
        }

        // ── Private Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Adds <paramref name="quantity"/> of <paramref name="itemId"/> to the player's inventory.
        /// If the item already exists, increments Quantity. Otherwise creates a new row.
        /// Must be called within an active transaction.
        /// </summary>
        private async Task UpsertInventoryItem(int playerProfileId, int itemId, int quantity)
        {
            var existing = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                _context.InventoryItems.Update(existing);
            }
            else
            {
                await _context.InventoryItems.AddAsync(new InventoryItem
                {
                    PlayerProfileId = playerProfileId,
                    ItemId = itemId,
                    Quantity = quantity,
                    IsEquipped = false,
                    IsSkin = false,
                    EnhancementLevel = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }
            // Note: SaveChanges is called once at the end of the transaction — not here.
        }

        /// <summary>Maps a Chest entity to its response DTO for chest preview.</summary>
        private static ChestResponseDto MapChestToDto(Chest chest)
        {
            return new ChestResponseDto
            {
                ChestId = chest.ChestId,
                Name = chest.Name,
                Description = chest.Description,
                Type = chest.Type,
                GoldMinReward = chest.GoldMinReward,
                GoldMaxReward = chest.GoldMaxReward,
                ExperienceReward = chest.ExperienceReward,
                IsActive = chest.IsActive,
                ChestItems = chest.ChestItems.Select(ci => new ChestItemResponseDto
                {
                    ChestItemId = ci.ChestItemId,
                    ChestId = ci.ChestId,
                    ItemId = ci.ItemId,
                    ItemName = ci.Item?.Name,
                    ItemIconUrl = ci.Item?.IconUrl,
                    ItemRarity = ci.Item?.Rarity,
                    QuantityMin = ci.QuantityMin,
                    QuantityMax = ci.QuantityMax,
                    DropRate = ci.DropRate,
                    IsGuaranteed = ci.IsGuaranteed
                }).ToList()
            };
        }
    }
}
