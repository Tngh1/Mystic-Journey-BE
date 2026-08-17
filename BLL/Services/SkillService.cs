using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BLL.Services
{
    // Executes core business logic for i skill service.
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IInventoryRepository _inventoryRepository;

        // Initialize this instance from repository, mapper, player profile repository, and transaction manager and store repository, mapper, player profile repository, transaction manager, and inventory repository for later operations.
        public SkillService(
            ISkillRepository repository,
            IMapper mapper,
            IPlayerProfileRepository playerProfileRepository,
            ITransactionManager transactionManager,
            IInventoryRepository inventoryRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _playerProfileRepository = playerProfileRepository;
            _transactionManager = transactionManager;
            _inventoryRepository = inventoryRepository;
        }

        // Executes core business logic for get skill by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed SkillResponseDto? result asynchronously.
        public async Task<SkillResponseDto?> GetSkillById(int id)
        {
            var skill = await _repository.GetSkillById(id);
            if (skill == null) return null;  // Entity not found — short-circuit with appropriate error result
            return _mapper.Map<SkillResponseDto>(skill);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get skills paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<SkillResponseDto result asynchronously.
        public async Task<PagedResultDto<SkillResponseDto>> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var (totalCount, items) = await _repository.GetSkillsPaged(page, pageSize, search, type, isActive);
            var dtos = items.Select(s => _mapper.Map<SkillResponseDto>(s)).ToList();  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<SkillResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for create skill.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed SkillResponseDto result asynchronously.
        public async Task<SkillResponseDto> CreateSkill(CreateSkillRequestDto request)
        {
            var skill = _mapper.Map<Skill>(request);  // Transform domain entity into DTO for the API response layer
            var created = await _repository.CreateSkill(skill);
            return _mapper.Map<SkillResponseDto>(created);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for update skill.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed SkillResponseDto result asynchronously.
        public async Task<SkillResponseDto> UpdateSkill(int id, UpdateSkillRequestDto request)
        {
            var skill = await _repository.GetSkillById(id)
                ?? throw new KeyNotFoundException($"Skill with id {id} not found.");

            skill.Name = request.Name;
            skill.Description = request.Description;
            skill.ImageUrl = request.ImageUrl;
            skill.Type = request.Type;
            skill.DamageType = request.DamageType;
            skill.TargetType = request.TargetType;
            skill.ClassRequirement = request.ClassRequirement;
            skill.CooldownSeconds = request.CooldownSeconds;
            skill.BaseDamage = request.BaseDamage;
            skill.DamagePerLevel = request.DamagePerLevel;
            skill.DamageGrowthPercent = request.DamageGrowthPercent;
            skill.UnlockLevel = request.UnlockLevel;
            skill.CorruptionCost = request.CorruptionCost;
            skill.IsActive = request.IsActive;

            var updated = await _repository.UpdateSkill(skill);
            return _mapper.Map<SkillResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for upgrade player skill.
        // Logic details: delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException, UnauthorizedAccessException on invalid state or rule violations.
        // Returns the computed PlayerSkillResponseDto result asynchronously.
        public async Task<PlayerSkillResponseDto> UpgradePlayerSkill(int actorPlayerProfileId, UpgradePlayerSkillRequestDto request)
        {
            var ps = await _repository.GetPlayerSkillById(request.PlayerSkillId)
                ?? throw new KeyNotFoundException("PlayerSkill not found.");

            if (ps.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("PlayerSkill does not belong to actor.");  // Authentication token is invalid or expired

            var playerProfile = await _playerProfileRepository.GetPlayerProfileById(actorPlayerProfileId);
            if (playerProfile == null) throw new InvalidOperationException("Player profile not found.");  // Entity not found — short-circuit with appropriate error result

            if (ps.Level >= playerProfile.Level)
            {
                throw new InvalidOperationException($"Skill level ({ps.Level}) cannot exceed player level ({playerProfile.Level}).");  // Unexpected runtime state — propagate to global error handler
            }

            int requiredStones = ps.Level;
            var inventoryItems = await _inventoryRepository.GetByPlayerId(actorPlayerProfileId);
            var stoneItem = inventoryItems.FirstOrDefault(i => i.Item?.Name == "Skill Upgrade Stone");

            if (stoneItem == null || stoneItem.Quantity < requiredStones)
            {
                throw new InvalidOperationException($"Not enough Skill Upgrade Stones. Required: {requiredStones}, Have: {(stoneItem?.Quantity ?? 0)}.");  // Unexpected runtime state — propagate to global error handler
            }

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                stoneItem.Quantity -= requiredStones;
                if (stoneItem.Quantity <= 0)
                {
                    await _inventoryRepository.DeleteItem(stoneItem.InventoryItemId);
                }
                else
                {
                    await _inventoryRepository.UpdateItem(stoneItem);
                }

                ps.Level += 1;
                ps.Experience = 0;

                var updated = await _repository.UpdatePlayerSkill(ps);

                return _mapper.Map<PlayerSkillResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
            });
        }

        // Executes core business logic for equip player skill.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException, UnauthorizedAccessException on invalid state or rule violations.
        // Returns the computed PlayerSkillResponseDto result asynchronously.
        public async Task<PlayerSkillResponseDto> EquipPlayerSkill(int actorPlayerProfileId, EquipSkillRequestDto request)
        {
            var ps = await _repository.GetPlayerSkillById(request.PlayerSkillId)
                ?? throw new KeyNotFoundException("PlayerSkill not found.");

            if (ps.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("PlayerSkill does not belong to actor.");  // Authentication token is invalid or expired

            var playerProfile = await _playerProfileRepository.GetPlayerProfileById(actorPlayerProfileId);
            var skillDef = ps.Skill ?? await _repository.GetSkillById(ps.SkillId);
            if (playerProfile != null && skillDef != null)
            {
                if (!string.IsNullOrWhiteSpace(skillDef.ClassRequirement) && !string.IsNullOrWhiteSpace(playerProfile.Class))
                {
                    if (!skillDef.ClassRequirement.Equals("All", StringComparison.OrdinalIgnoreCase) &&
                        !skillDef.ClassRequirement.Equals(playerProfile.Class, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException($"Cannot equip skill: requires class {skillDef.ClassRequirement}.");  // Unexpected runtime state — propagate to global error handler
                    }
                }
            }
            if (ps.NextAvailableTime.HasValue && ps.NextAvailableTime.Value > DateTime.UtcNow)
                throw new InvalidOperationException("Cannot change a skill slot while the skill is on cooldown.");  // Unexpected runtime state — propagate to global error handler


            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                if (request.IsEquipped)
                {
                    // Supported equipment slots: None, Weapon, Armor, Helmet, Gloves, Boots, Ring, Necklace, or Shield.
                    var slot = request.SlotIndex ?? 0;
                    if (slot < 0 || slot > 2) throw new ArgumentException("Invalid slot index.");

                    var requiredLevel = slot switch { 0 => 1, 1 => 5, 2 => 10, _ => int.MaxValue };
                    if (playerProfile == null || playerProfile.Level < requiredLevel)
                        throw new InvalidOperationException($"Skill slot {slot + 1} unlocks at level {requiredLevel}.");  // Unexpected runtime state — propagate to global error handler

                    var others = await _repository.GetPlayerSkillsByPlayerId(actorPlayerProfileId);
                    foreach (var other in others)
                    {
                        if (other.PlayerSkillId != ps.PlayerSkillId && other.EquippedSlot.HasValue && other.EquippedSlot.Value == slot)
                        {
                            other.EquippedSlot = null;
                            await _repository.UpdatePlayerSkill(other);
                        }
                    }

                    foreach (var otherSameSkill in others)
                    {
                        if (otherSameSkill.PlayerSkillId != ps.PlayerSkillId && otherSameSkill.SkillId == ps.SkillId && otherSameSkill.EquippedSlot.HasValue)
                        {
                            otherSameSkill.EquippedSlot = null;
                            await _repository.UpdatePlayerSkill(otherSameSkill);
                        }
                    }

                    ps.EquippedSlot = slot;
                }
                else
                {
                    ps.EquippedSlot = null;
                }

                var updated = await _repository.UpdatePlayerSkill(ps);

                return _mapper.Map<PlayerSkillResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
            });
        }

        // Executes core business logic for record skill cast.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws InvalidOperationException, KeyNotFoundException, UnauthorizedAccessException on invalid state or rule violations.
        // Returns the computed PlayerSkillResponseDto result asynchronously.
        public async Task<PlayerSkillResponseDto> RecordSkillCast(int actorPlayerProfileId, int playerSkillId)
        {
            var ps = await _repository.GetPlayerSkillById(playerSkillId)
                ?? throw new KeyNotFoundException("PlayerSkill not found.");

            if (ps.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("PlayerSkill does not belong to actor.");  // Authentication token is invalid or expired

            var skillDef = ps.Skill ?? await _repository.GetSkillById(ps.SkillId);
            if (skillDef == null) throw new InvalidOperationException("Skill definition not found.");  // Entity not found — short-circuit with appropriate error result

            ps.NextAvailableTime = DateTime.UtcNow.AddSeconds(skillDef.CooldownSeconds);
            var updated = await _repository.UpdatePlayerSkill(ps);
            return _mapper.Map<PlayerSkillResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for dismantle player skill.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; throws InvalidOperationException, KeyNotFoundException, UnauthorizedAccessException on invalid state or rule violations.
        // Returns the computed PlayerSkillResponseDto? result asynchronously.
        public async Task<PlayerSkillResponseDto?> DismantlePlayerSkill(int actorPlayerProfileId, DismantlePlayerSkillRequestDto request)
        {
            var ps = await _repository.GetPlayerSkillById(request.PlayerSkillId)
                ?? throw new KeyNotFoundException("PlayerSkill not found.");

            if (ps.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("PlayerSkill does not belong to actor.");  // Authentication token is invalid or expired

            if (ps.EquippedSlot.HasValue)
                throw new InvalidOperationException("Cannot dismantle an equipped skill. Please unequip it first.");  // Unexpected runtime state — propagate to global error handler

            int xpGranted = 100 * Math.Max(1, ps.Level);

            PlayerSkill? target = null;
            if (request.TargetPlayerSkillId.HasValue)
            {
                target = await _repository.GetPlayerSkillById(request.TargetPlayerSkillId.Value)
                    ?? throw new KeyNotFoundException("Target PlayerSkill not found.");

                if (target.PlayerProfileId != actorPlayerProfileId)
                    throw new UnauthorizedAccessException("Target PlayerSkill does not belong to actor.");  // Authentication token is invalid or expired

                var playerProfile = await _playerProfileRepository.GetPlayerProfileById(actorPlayerProfileId);
                var targetSkillDef = target.Skill ?? await _repository.GetSkillById(target.SkillId);
                if (playerProfile != null && targetSkillDef != null && !string.IsNullOrWhiteSpace(targetSkillDef.ClassRequirement))
                {
                    if (!targetSkillDef.ClassRequirement.Equals(playerProfile.Class, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Target skill is not usable by player's class.");  // Unexpected runtime state — propagate to global error handler
                }

                target.Experience += xpGranted;
                while (target.Experience >= 100 * target.Level)
                {
                    target.Experience -= 100 * target.Level;
                    target.Level += 1;
                }

                await _repository.UpdatePlayerSkill(target);
            }

            await _repository.DeletePlayerSkill(ps);

            if (target != null)  // Entity exists — proceed with conditional branch
            {
                return _mapper.Map<PlayerSkillResponseDto>(target);  // Transform domain entity into DTO for the API response layer
            }

            return null;
        }


        // Executes core business logic for get me skills.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PlayerMeSkillsResponseDto result asynchronously.
        public async Task<PlayerMeSkillsResponseDto> GetMeSkills(int playerProfileId)
        {
            var skillsList = await _repository.GetPlayerSkillsByPlayerId(playerProfileId);

            var skills = _mapper.Map<List<PlayerSkillResponseDto>>(skillsList);  // Transform domain entity into DTO for the API response layer

            return new PlayerMeSkillsResponseDto
            {
                PlayerProfileId = playerProfileId,
                Skills = skills,
                TotalCount = skills.Count
            };
        }
    }
}
