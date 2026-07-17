using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
    public class DungeonSessionService : IDungeonSessionService
    {
        private readonly IDungeonConfigRepository _dungeonConfigRepository;
        private readonly IDungeonSessionRepository _sessionRepository;
        private readonly IDungeonProgressRepository _progressRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IPlayerProfileService _playerProfileService;
        private readonly ITransactionManager _transactionManager;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public DungeonSessionService(
            IDungeonConfigRepository dungeonConfigRepository,
            IDungeonSessionRepository sessionRepository,
            IDungeonProgressRepository progressRepository,
            IPlayerProfileRepository profileRepository,
            IPlayerProfileService playerProfileService,
            ITransactionManager transactionManager,
            IInventoryRepository inventoryRepository,
            IMapper mapper)
        {
            _dungeonConfigRepository = dungeonConfigRepository;
            _sessionRepository = sessionRepository;
            _progressRepository = progressRepository;
            _profileRepository = profileRepository;
            _playerProfileService = playerProfileService;
            _transactionManager = transactionManager;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
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
            // By abandoning the old session and starting a new one.
            var existing = await _sessionRepository.GetActiveSession(playerProfileId, dungeonConfigId);
            if (existing != null)
            {
                existing.Status = "Failed";
                await _sessionRepository.Update(existing);
            }

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
                progress.BossSpawned = request.BossSpawned;
                progress.ElapsedTime = request.ElapsedTime;
                await _progressRepository.Create(progress);
            }
            else
            {
                progress.MonstersKilled = request.MonstersKilled;
                progress.BossKilled = request.BossKilled;
                progress.CompletionPercentage = request.CompletionPercentage;
                progress.ExtraData = request.ExtraData;
                progress.BossSpawned = request.BossSpawned;
                progress.ElapsedTime = request.ElapsedTime;
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
                BossSpawned = progress.BossSpawned,
                ElapsedTime = progress.ElapsedTime,
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
                chestDto = _mapper.Map<ChestResponseDto>(session.DungeonConfig.Chest);

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

            // Guard against duplicate claims
            if (session.IsRewardClaimed)
                throw new InvalidOperationException("CONFLICT: Rewards have already been claimed for this session.");

            // BR-08: Session must be Completed
            if (session.Status != "Completed")
                throw new InvalidOperationException(
                    $"Session {sessionId} cannot have rewards claimed (status: {session.Status}). Complete the dungeon first.");

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

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
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
                        bool isEquipment = chestItem.Item?.Type?.Equals("Equipment", StringComparison.OrdinalIgnoreCase) == true;
                        if (isEquipment)
                        {
                            // Add equipment as independent entries
                            for (int i = 0; i < quantity; i++)
                            {
                                await UpsertInventoryItem(playerProfileId, chestItem.ItemId, 1, isEquipment);
                            }
                        }
                        else
                        {
                            await UpsertInventoryItem(playerProfileId, chestItem.ItemId, quantity, isEquipment);
                        }

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
                profile.AddExperience(experienceEarned);
                profile.UpdatedAt = DateTime.UtcNow;
                await _profileRepository.UpdatePlayerProfile(profile);

                // Step 6 — Mark session as RewardClaimed
                session.IsRewardClaimed = true;
                session.Status = "RewardClaimed";
                session.ClaimedAt = DateTime.UtcNow;
                session.UpdatedAt = DateTime.UtcNow;
                await _sessionRepository.Update(session);

                var timeTakenSeconds = session.CompletedTime.HasValue 
                    ? (float)(session.CompletedTime.Value - session.EnterTime).TotalSeconds 
                    : 0f;

                return new ClaimDungeonRewardResponseDto
                {
                    DungeonSessionId = sessionId,
                    Success = true,
                    Message = "Rewards claimed successfully!",
                    EnergyConsumed = dungeon.EnergyCost,
                    GoldEarned = goldEarned,
                    ExperienceEarned = experienceEarned,
                    TimeTakenSeconds = timeTakenSeconds,
                    Items = rewardItems,
                    Wallet = new WalletDto
                    {
                        Gold = profile.Gold,
                        Gems = profile.Gems
                    },
                    Character = new CharacterDto
                    {
                        Level = profile.Level,
                        ExperiencePoints = profile.ExperiencePoints,
                        Energy = profile.CurrentEnergy,
                        MaxEnergy = profile.MaxEnergy
                    }
                };
            });
        }

        // ── 5. Abandon Session ────────────────────────────────────────────────────────
        
        public async Task<bool> AbandonSession(int sessionId, int playerProfileId)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            if (session.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You do not own this dungeon session.");

            if (session.Status != "Active")
                throw new InvalidOperationException($"Session {sessionId} is not active. Status: {session.Status}");

            session.Status = "Abandoned";
            session.UpdatedAt = DateTime.UtcNow;
            await _sessionRepository.Update(session);

            return true;
        }

        // ── 6. Get Active Session ──────────────────────────────────────────────────────
        
        public async Task<EnterDungeonResponseDto?> GetActiveSession(int playerProfileId)
        {
            var session = await _sessionRepository.GetActiveSession(playerProfileId, null);
            if (session == null) return null;

            return new EnterDungeonResponseDto
            {
                DungeonSessionId = session.DungeonSessionId,
                PlayerProfileId = playerProfileId,
                DungeonConfigId = session.DungeonConfigId,
                DungeonName = session.DungeonConfig?.Name ?? "Unknown",
                EnergyCost = session.DungeonConfig?.EnergyCost ?? 0,
                PlayerCurrentEnergy = 0, // Not critical for resume
                EnterTime = session.EnterTime,
                Status = session.Status,
                PartyMembers = string.IsNullOrEmpty(session.PartyMembers) 
                    ? new List<string>() 
                    : session.PartyMembers.Split(',').ToList(),
                Progress = session.Progress != null ? new DungeonProgressResponseDto
                {
                    DungeonProgressId = session.Progress.DungeonProgressId,
                    DungeonSessionId = session.DungeonSessionId,
                    MonstersKilled = session.Progress.MonstersKilled,
                    BossSpawned = session.Progress.BossSpawned,
                    BossKilled = session.Progress.BossKilled,
                    ElapsedTime = session.Progress.ElapsedTime,
                    CompletionPercentage = session.Progress.CompletionPercentage,
                    ExtraData = session.Progress.ExtraData,
                    UpdatedAt = session.Progress.UpdatedAt,
                    SessionStatus = session.Status
                } : null
            };
        }

        // ── 7. Get History ───────────────────────────────────────────────────────────

        public async Task<List<DungeonHistoryResponseDto>> GetHistory(int playerProfileId)
        {
            // Pull all sessions for the player, filter for completed ones
            var allSessions = await _sessionRepository.GetByPlayerProfileId(playerProfileId);
            
            var history = allSessions
                .Where(s => s.Status == "Completed" || s.Status == "RewardClaimed")
                .Select(s => new DungeonHistoryResponseDto
                {
                    DungeonSessionId = s.DungeonSessionId,
                    DungeonName = s.DungeonConfig?.Name ?? "Unknown",
                    Difficulty = s.DungeonConfig?.Difficulty ?? 1,
                    Status = s.Status,
                    ElapsedTime = s.Progress?.ElapsedTime ?? 0,
                    CompletionPercentage = s.Progress?.CompletionPercentage ?? 0,
                    EnterTime = s.EnterTime,
                    CompletedTime = s.CompletedTime ?? s.ClaimedAt ?? s.UpdatedAt
                })
                .ToList();

            return history;
        }

        // ── Private Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Adds <paramref name="quantity"/> of <paramref name="itemId"/> to the player's inventory.
        /// If the item already exists and is not equipment, increments Quantity. Otherwise creates a new row.
        /// Must be called within an active transaction.
        /// </summary>
        private async Task UpsertInventoryItem(int playerProfileId, int itemId, int quantity, bool isEquipment)
        {
            if (!isEquipment)
            {
                var existing = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, itemId);

                if (existing != null)
                {
                    existing.Quantity += quantity;
                    await _inventoryRepository.UpdateItem(existing);
                    return;
                }
            }

            // Either it's equipment (always new row) or it's a new stackable item
            await _inventoryRepository.AddItem(new InventoryItem
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


    }
}
