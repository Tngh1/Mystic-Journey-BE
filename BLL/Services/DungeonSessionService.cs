using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace BLL.Services
{
    // Executes core business logic for i dungeon session service.
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
        private readonly IRewardDeliveryService _rewardDeliveryService;

        // Initialize this instance from dungeon config repository, session repository, progress repository, and profile repository and store dungeon config repository, session repository, progress repository, profile repository, and player profile service for later operations.
        public DungeonSessionService(
            IDungeonConfigRepository dungeonConfigRepository,
            IDungeonSessionRepository sessionRepository,
            IDungeonProgressRepository progressRepository,
            IPlayerProfileRepository profileRepository,
            IPlayerProfileService playerProfileService,
            ITransactionManager transactionManager,
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            IRewardDeliveryService rewardDeliveryService)
        {
            _dungeonConfigRepository = dungeonConfigRepository;
            _sessionRepository = sessionRepository;
            _progressRepository = progressRepository;
            _profileRepository = profileRepository;
            _playerProfileService = playerProfileService;
            _transactionManager = transactionManager;
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _rewardDeliveryService = rewardDeliveryService;
        }


        // Executes core business logic for enter dungeon.
        // Logic details: delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed EnterDungeonResponseDto result asynchronously.
        public async Task<EnterDungeonResponseDto> EnterDungeon(int playerProfileId, int dungeonConfigId, List<string>? partyMembers = null)
        {
            var profile = await _profileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile {playerProfileId} not found.");

            _playerProfileService.RecalculateEnergy(profile);
            await _profileRepository.UpdatePlayerProfile(profile);

            var dungeon = await _dungeonConfigRepository.GetByIdWithChest(dungeonConfigId)
                ?? throw new KeyNotFoundException($"Dungeon {dungeonConfigId} not found or is not active.");


            if (profile.Level < dungeon.LevelRequirement)
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    $"Level {dungeon.LevelRequirement} required to enter {dungeon.Name}. You are level {profile.Level}.");

            int totalMembersCount = 1 + (partyMembers?.Count ?? 0);
            if (totalMembersCount > dungeon.MaxMembers)
                throw new InvalidOperationException($"Party exceeds maximum allowed size of {dungeon.MaxMembers}.");  // Unexpected runtime state — propagate to global error handler

            await _sessionRepository.FailActiveSessions(playerProfileId);

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


        // Update progress using session id, player profile id, and request; it loads by id, creates create, and updates update and guards invalid or unavailable states.
        public async Task<DungeonProgressResponseDto> UpdateProgress(
            int sessionId, int playerProfileId, UpdateDungeonProgressRequestDto request)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            if (session.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You do not own this dungeon session.");  // Authentication token is invalid or expired

            if (session.Status != "Active")
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    $"Session {sessionId} is not active (current status: {session.Status}). Progress can only be updated for active sessions.");

            var progress = session.Progress;
            if (progress == null)  // Entity not found — short-circuit with appropriate error result
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


        // Executes core business logic for complete session.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed CompleteDungeonResponseDto result asynchronously.
        public async Task<CompleteDungeonResponseDto> CompleteSession(int sessionId, int playerProfileId)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            if (session.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You do not own this dungeon session.");  // Authentication token is invalid or expired

            if (session.Status != "Active")
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    $"Session {sessionId} cannot be completed (current status: {session.Status}).");

            var progress = session.Progress;
            if (progress == null || !progress.BossKilled)
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    "The dungeon boss has not been defeated yet. Defeat the boss to complete the dungeon.");

            session.Status = "Completed";
            session.CompletedTime = DateTime.UtcNow;
            await _sessionRepository.Update(session);

            ChestResponseDto? chestDto = null;
            if (session.DungeonConfig?.Chest != null)
                chestDto = _mapper.Map<ChestResponseDto>(session.DungeonConfig.Chest);  // Transform domain entity into DTO for the API response layer

            return new CompleteDungeonResponseDto
            {
                DungeonSessionId = sessionId,
                Status = session.Status,
                CompletedTime = session.CompletedTime!.Value,
                RewardChest = chestDto,
                Message = "Dungeon completed! Call claim-reward to collect your rewards."
            };
        }


        // Executes core business logic for claim reward.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed ClaimDungeonRewardResponseDto result asynchronously.
        public async Task<ClaimDungeonRewardResponseDto> ClaimReward(int sessionId, int playerProfileId)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            bool isOwner = session.PlayerProfileId == playerProfileId;
            bool isPartyMember = false;

            if (!string.IsNullOrEmpty(session.PartyMembers))
            {
                var partyIds = session.PartyMembers.Split(',');
                isPartyMember = partyIds.Contains(playerProfileId.ToString());
            }

            if (!isOwner && !isPartyMember)
                throw new UnauthorizedAccessException("You do not own this dungeon session and you are not a party member.");  // Authentication token is invalid or expired

            bool hasClaimed = false;
            if (!string.IsNullOrEmpty(session.ClaimedByMembers))
            {
                var claimedIds = session.ClaimedByMembers.Split(',');
                hasClaimed = claimedIds.Contains(playerProfileId.ToString());
            }

            if (hasClaimed)
                throw new InvalidOperationException("CONFLICT: Rewards have already been claimed by this player for this session.");  // Unexpected runtime state — propagate to global error handler

            if (session.Status != "Completed")
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    $"Session {sessionId} cannot have rewards claimed (status: {session.Status}). Complete the dungeon first.");

            var profile = await _profileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile {playerProfileId} not found.");

            _playerProfileService.RecalculateEnergy(profile);

            var dungeon = session.DungeonConfig
                ?? throw new InvalidOperationException("Dungeon configuration is missing from session.");  // Unexpected runtime state — propagate to global error handler

            if (profile.CurrentEnergy < dungeon.EnergyCost)
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    $"Insufficient energy to claim reward. Required: {dungeon.EnergyCost}, Current: {profile.CurrentEnergy}.");

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                profile.CurrentEnergy -= dungeon.EnergyCost;
                profile.TotalDungeonClears += 1;

                var chest = dungeon.Chest;
                var goldEarned = 0;
                var experienceEarned = 0;
                var rewardItems = new List<DungeonRewardItemDto>();

                if (chest != null)  // Entity exists — proceed with conditional branch
                {
                    goldEarned = chest.GoldMaxReward > chest.GoldMinReward
                        ? Random.Shared.Next(chest.GoldMinReward, chest.GoldMaxReward + 1)
                        : chest.GoldMinReward;

                    experienceEarned = chest.ExperienceReward;

                    foreach (var chestItem in chest.ChestItems)
                    {
                        var drops = chestItem.IsGuaranteed ||
                                    Random.Shared.NextDouble() * 100 <= (double)chestItem.DropRate;
                        if (!drops) continue;

                        var quantity = chestItem.QuantityMax > chestItem.QuantityMin
                            ? Random.Shared.Next(chestItem.QuantityMin, chestItem.QuantityMax + 1)
                            : chestItem.QuantityMin;

                        bool isEquipment = chestItem.Item?.Type?.Equals("Equipment", StringComparison.OrdinalIgnoreCase) == true;
                        if (isEquipment)
                        {
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

                profile.Gold += goldEarned;
                profile.AddExperience(experienceEarned);
                profile.UpdatedAt = DateTime.UtcNow;
                await _profileRepository.UpdatePlayerProfile(profile);

                session.IsRewardClaimed = true;
                if (string.IsNullOrEmpty(session.ClaimedByMembers))  // Mandatory string argument is null or empty — fail fast
                {
                    session.ClaimedByMembers = playerProfileId.ToString();
                }
                else
                {
                    session.ClaimedByMembers += "," + playerProfileId;
                }

                session.ClaimedAt = DateTime.UtcNow;
                session.IsRewardClaimed = true;

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


        // Executes core business logic for abandon session.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed bool result asynchronously.
        public async Task<bool> AbandonSession(int sessionId, int playerProfileId)
        {
            var session = await _sessionRepository.GetById(sessionId)
                ?? throw new KeyNotFoundException($"Dungeon session {sessionId} not found.");

            if (session.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You do not own this dungeon session.");  // Authentication token is invalid or expired

            if (session.Status != "Active")
                throw new InvalidOperationException($"Session {sessionId} is not active. Status: {session.Status}");  // Unexpected runtime state — propagate to global error handler

            session.Status = "Abandoned";
            session.UpdatedAt = DateTime.UtcNow;
            await _sessionRepository.Update(session);

            return true;
        }


        // Executes core business logic for get active session.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer.
        // Returns the computed EnterDungeonResponseDto? result asynchronously.
        public async Task<EnterDungeonResponseDto?> GetActiveSession(int playerProfileId)
        {
            var session = await _sessionRepository.GetActiveSession(playerProfileId, null);
            if (session == null) return null;  // Entity not found — short-circuit with appropriate error result

            return new EnterDungeonResponseDto
            {
                DungeonSessionId = session.DungeonSessionId,
                PlayerProfileId = playerProfileId,
                DungeonConfigId = session.DungeonConfigId,
                DungeonName = session.DungeonConfig?.Name ?? "Unknown",
                EnergyCost = session.DungeonConfig?.EnergyCost ?? 0,
                PlayerCurrentEnergy = 0,
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


        // Executes core business logic for get history.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed List<DungeonHistoryResponseDto result asynchronously.
        public async Task<List<DungeonHistoryResponseDto>> GetHistory(int playerProfileId)
        {
            var allSessions = await _sessionRepository.GetByPlayerProfileId(playerProfileId);

            var history = allSessions
                .Where(s => s.Status == "Completed" || s.Status == "RewardClaimed")  // Filter records matching the predicate
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


        // Executes core business logic for upsert inventory item.
        // Completes asynchronously upon successful execution.
        private async Task UpsertInventoryItem(int playerProfileId, int itemId, int quantity, bool isEquipment)
            => await _rewardDeliveryService.DeliverItemAsync(playerProfileId, itemId, quantity, "dungeon reward");


    }
}
