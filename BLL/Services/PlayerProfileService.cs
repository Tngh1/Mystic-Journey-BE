using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class PlayerProfileService : IPlayerProfileService
    {
        private readonly IPlayerProfileRepository _repository;
        private readonly IMapper _mapper;

        public PlayerProfileService(IPlayerProfileRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PlayerProfileDetailResponseDto?> GetProfileById(int id)
        {
            var profile = await _repository.GetPlayerProfileByIdWithStats(id);
            if (profile == null)
                return null;

            return MapToDetailResponseDto(profile);
        }

        public async Task<PlayerProfileResponseDto> UpdateProfile(int id, UpdatePlayerProfileRequestDto request)
        {
            var profile = await _repository.GetPlayerProfileById(id)
                ?? throw new KeyNotFoundException($"Player profile with id {id} not found.");

            if (request.DisplayName != null)
                profile.DisplayName = request.DisplayName;

            if (request.AvatarUrl != null)
                profile.AvatarUrl = request.AvatarUrl;

            if (request.PlayerClass != null)
                profile.Class = request.PlayerClass;

            if (request.Level > 0)
                profile.Level = request.Level;

            if (request.ExperiencePoints >= 0)
                profile.ExperiencePoints = request.ExperiencePoints;

            if (request.Gold >= 0)
                profile.Gold = request.Gold;

            if (request.Gems >= 0)
                profile.Gems = request.Gems;

            if (request.Energy >= 0)
                profile.Energy = request.Energy;

            profile.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdatePlayerProfile(profile);
            return MapToResponseDto(updated);
        }

        public async Task<PagedResultDto<PlayerProfileResponseDto>> GetProfilesPaged(int page, int pageSize, string? search, int? level)
        {
            var (totalCount, items) = await _repository.GetProfilesPaged(page, pageSize, search, level);
            var dtos = items.Select(MapToResponseDto).ToList();
            return new PagedResultDto<PlayerProfileResponseDto>(totalCount, dtos);
        }

        public async Task<PlayerMeInventoryResponseDto> GetMeInventory(int playerProfileId)
        {
            var profile = await _repository.GetByIdWithAll(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile with id {playerProfileId} not found.");

            var items = profile.InventoryItems?.Select(i => new InventoryItemResponseDto
            {
                InventoryItemId = i.InventoryItemId,
                PlayerProfileId = i.PlayerProfileId,
                ItemId = i.ItemId,
                ItemName = i.Item?.Name ?? "",
                ItemDescription = i.Item?.Description,
                ItemType = i.Item?.Type ?? "",
                ItemRarity = i.Item?.Rarity ?? "",
                ItemSlot = i.Item?.Slot ?? "None",
                IconUrl = i.Item?.IconUrl,
                Quantity = i.Quantity,
                IsEquipped = i.IsEquipped,
                IsSkin = i.IsSkin,
                EquippedSlot = i.EquippedSlot,
                EnhancementLevel = i.EnhancementLevel,
                CreatedAt = i.CreatedAt
            }).ToList() ?? new();

            return new PlayerMeInventoryResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                Items = items,
                TotalCount = items.Count
            };
        }

        public async Task<PlayerMeSkillsResponseDto> GetMeSkills(int playerProfileId)
        {
            var profile = await _repository.GetByIdWithAll(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile with id {playerProfileId} not found.");

            var skills = profile.PlayerSkills?.Select(ps => new PlayerSkillResponseDto
            {
                PlayerSkillId = ps.PlayerSkillId,
                PlayerProfileId = ps.PlayerProfileId,
                SkillId = ps.SkillId,
                SkillName = ps.Skill?.Name ?? "",
                SkillDescription = ps.Skill?.Description,
                SkillType = ps.Skill?.Type ?? "",
                DamageType = ps.Skill?.DamageType ?? "",
                TargetType = ps.Skill?.TargetType ?? "",
                Level = ps.Level,
                Experience = ps.Experience,
                IsEquipped = ps.IsEquipped,
                CooldownSeconds = ps.Skill?.CooldownSeconds ?? 0,
                BaseDamage = ps.Skill?.BaseDamage ?? 0,
                UnlockLevel = ps.Skill?.UnlockLevel ?? 1,
                UnlockedAt = ps.UnlockedAt
            }).ToList() ?? new();

            return new PlayerMeSkillsResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                Skills = skills,
                TotalCount = skills.Count
            };
        }

        public async Task<PlayerMeQuestsResponseDto> GetMeQuests(int playerProfileId)
        {
            var profile = await _repository.GetByIdWithAll(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile with id {playerProfileId} not found.");

            var quests = profile.PlayerQuests?.Select(pq => new PlayerQuestResponseDto
            {
                PlayerQuestId = pq.PlayerQuestId,
                PlayerProfileId = pq.PlayerProfileId,
                QuestId = pq.QuestId,
                QuestTitle = pq.Quest?.Title ?? "",
                QuestDescription = pq.Quest?.Description,
                QuestType = pq.Quest?.Type ?? "",
                MapName = pq.Quest?.MapName ?? "",
                RegionName = pq.Quest?.RegionName,
                ObjectiveType = pq.Quest?.ObjectiveType ?? "",
                ObjectiveTarget = pq.Quest?.ObjectiveTarget,
                ObjectiveLocation = pq.Quest?.ObjectiveLocation,
                QuestGiverName = pq.Quest?.QuestGiverName,
                Status = pq.Status,
                Progress = pq.Progress,
                TargetValue = pq.TargetValue,
                TargetAmount = pq.Quest?.TargetAmount ?? 0,
                RequiredLevel = pq.Quest?.RequiredLevel ?? 0,
                RewardExperience = pq.Quest?.RewardExperience ?? 0,
                RewardGold = pq.Quest?.RewardGold ?? 0,
                RewardGems = pq.Quest?.RewardGems ?? 0,
                RewardItemId = pq.Quest?.RewardItemId,
                RewardItemName = pq.Quest?.RewardItem?.Name,
                AcceptedAt = pq.AcceptedAt,
                CompletedAt = pq.CompletedAt,
                ClaimedAt = pq.ClaimedAt
            }).ToList() ?? new();

            return new PlayerMeQuestsResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                Quests = quests,
                TotalCount = quests.Count
            };
        }

        public async Task<PlayerMeAchievementsResponseDto> GetMeAchievements(int playerProfileId)
        {
            var profile = await _repository.GetByIdWithAll(playerProfileId)
                ?? throw new KeyNotFoundException($"Player profile with id {playerProfileId} not found.");

            var achievements = profile.PlayerAchievements ?? new List<PlayerAchievement>();

            var dtos = achievements.Select(pa => new PlayerAchievementResponseDto
            {
                PlayerAchievementId = pa.PlayerAchievementId,
                PlayerProfileId = pa.PlayerProfileId,
                AchievementId = pa.AchievementId,
                AchievementName = pa.Achievement?.Name ?? "",
                AchievementDescription = pa.Achievement?.Description,
                AchievementType = pa.Achievement?.Type ?? "",
                IconUrl = pa.Achievement?.IconUrl,
                Progress = pa.Progress,
                RequiredValue = pa.Achievement?.RequiredValue ?? 0,
                IsCompleted = pa.IsCompleted,
                CompletedAt = pa.CompletedAt,
                UnlockedAt = pa.UnlockedAt,
                RewardItemId = pa.Achievement?.RewardItemId,
                RewardItemName = pa.Achievement?.RewardItem?.Name,
                RewardQuantity = pa.Achievement?.RewardQuantity ?? 0,
                RewardGold = pa.Achievement?.RewardGold ?? 0,
                RewardGem = pa.Achievement?.RewardGem ?? 0
            }).ToList();

            return new PlayerMeAchievementsResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                Achievements = dtos,
                TotalCount = dtos.Count,
                CompletedCount = dtos.Count(a => a.IsCompleted)
            };
        }

        private static PlayerProfileResponseDto MapToResponseDto(PlayerProfile profile)
        {
            return new PlayerProfileResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                AccountId = profile.AccountId,
                AccountEmail = profile.Account?.Email,
                DisplayName = profile.DisplayName,
                AvatarUrl = string.IsNullOrEmpty(profile.AvatarUrl) ? null : profile.AvatarUrl,
                PlayerClass = profile.Class,
                Level = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        private static PlayerProfileDetailResponseDto MapToDetailResponseDto(PlayerProfile profile)
        {
            return new PlayerProfileDetailResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                AccountId = profile.AccountId,
                AccountEmail = profile.Account?.Email,
                DisplayName = profile.DisplayName,
                AvatarUrl = string.IsNullOrEmpty(profile.AvatarUrl) ? null : profile.AvatarUrl,
                PlayerClass = profile.Class,
                Level = profile.Level,
                ExperiencePoints = profile.ExperiencePoints,
                Gold = profile.Gold,
                Gems = profile.Gems,
                Energy = profile.Energy,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                Stats = profile.PlayerStats != null ? new PlayerStatsResponseDto
                {
                    CurrentHp = profile.PlayerStats.CurrentHp,
                    MaxHp = profile.PlayerStats.MaxHp,
                    Atk = profile.PlayerStats.Atk,
                    Def = profile.PlayerStats.Def,
                    MoveSpeed = profile.PlayerStats.MoveSpeed,
                    AttackSpeed = profile.PlayerStats.AttackSpeed,
                    CritRate = profile.PlayerStats.CritRate,
                    CritDamage = profile.PlayerStats.CritDamage,
                    DamageBonus = profile.PlayerStats.DamageBonus,
                    SkillPoints = profile.PlayerStats.SkillPoints,
                    TotalWins = profile.PlayerStats.TotalWins,
                    TotalLosses = profile.PlayerStats.TotalLosses,
                    TotalKills = profile.PlayerStats.TotalKills,
                    TotalDeaths = profile.PlayerStats.TotalDeaths
                } : null
            };
        }
    }
}
