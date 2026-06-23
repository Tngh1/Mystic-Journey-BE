using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace BLL.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _repository;
        private readonly IMapper _mapper;
        private readonly MysticJourneyDbContext _context;

        public SkillService(ISkillRepository repository, IMapper mapper, MysticJourneyDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<SkillResponseDto?> GetSkillById(int id)
        {
            var skill = await _repository.GetSkillById(id);
            if (skill == null) return null;
            return _mapper.Map<SkillResponseDto>(skill);
        }

        public async Task<PagedResultDto<SkillResponseDto>> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var (totalCount, items) = await _repository.GetSkillsPaged(page, pageSize, search, type, isActive);
            var dtos = items.Select(s => _mapper.Map<SkillResponseDto>(s)).ToList();
            return new PagedResultDto<SkillResponseDto>(totalCount, dtos);
        }

        public async Task<SkillResponseDto> CreateSkill(CreateSkillRequestDto request)
        {
            var skill = _mapper.Map<Skill>(request);
            var created = await _repository.CreateSkill(skill);
            return _mapper.Map<SkillResponseDto>(created);
        }

        public async Task<SkillResponseDto> UpdateSkill(int id, UpdateSkillRequestDto request)
        {
            var skill = await _repository.GetSkillById(id)
                ?? throw new KeyNotFoundException($"Skill with id {id} not found.");

            skill.Name = request.Name;
            skill.Description = request.Description;
            skill.Type = request.Type;
            skill.DamageType = request.DamageType;
            skill.TargetType = request.TargetType;
            skill.ClassRequirement = request.ClassRequirement;
            skill.CooldownSeconds = request.CooldownSeconds;
            skill.BaseDamage = request.BaseDamage;
            skill.DamagePerLevel = request.DamagePerLevel;
            skill.DamageGrowthPercent = request.DamageGrowthPercent;
            skill.UnlockLevel = request.UnlockLevel;
            skill.IsActive = request.IsActive;

            var updated = await _repository.UpdateSkill(skill);
            return _mapper.Map<SkillResponseDto>(updated);
        }

        public async Task<PlayerSkillResponseDto> UpgradePlayerSkill(int actorPlayerProfileId, UpgradePlayerSkillRequestDto request)
        {
            var ps = await _repository.GetPlayerSkillById(request.PlayerSkillId)
                ?? throw new KeyNotFoundException("PlayerSkill not found.");

            if (ps.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("PlayerSkill does not belong to actor.");

            // simple upgrade logic: increase level by 1, reset experience
            ps.Level += 1;
            ps.Experience = 0;

            var updated = await _repository.UpdatePlayerSkill(ps);

            var dto = _mapper.Map<PlayerSkillResponseDto>(updated);
            dto.CooldownSeconds = updated.Skill?.CooldownSeconds ?? 0;
            dto.BaseDamage = updated.Skill?.BaseDamage ?? 0.0;
            dto.EffectiveDamage = (updated.Skill?.BaseDamage ?? 0.0) * (1 + (updated.Skill?.DamageGrowthPercent ?? 0.0) / 100.0 * (updated.Level - 1))
                + (updated.Skill?.DamagePerLevel ?? 0.0) * (updated.Level - 1);

            return dto;
        }

        public async Task<PlayerSkillResponseDto> EquipPlayerSkill(int actorPlayerProfileId, EquipSkillRequestDto request)
        {
            var ps = await _repository.GetPlayerSkillById(request.PlayerSkillId)
                ?? throw new KeyNotFoundException("PlayerSkill not found.");

            if (ps.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("PlayerSkill does not belong to actor.");

            // Enforce class requirement: player cannot equip skills not matching their selected class
            var playerProfile = await _context.PlayerProfiles.FindAsync(actorPlayerProfileId);
            var skillDef = ps.Skill ?? await _repository.GetSkillById(ps.SkillId);
            if (playerProfile != null && skillDef != null)
            {
                if (!string.IsNullOrWhiteSpace(skillDef.ClassRequirement) && !string.IsNullOrWhiteSpace(playerProfile.Class))
                {
                    // 👇 ĐÃ SỬA: Cho phép vượt qua nếu yêu cầu là "All"
                    if (!skillDef.ClassRequirement.Equals("All", StringComparison.OrdinalIgnoreCase) && 
                        !skillDef.ClassRequirement.Equals(playerProfile.Class, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException($"Cannot equip skill: requires class {skillDef.ClassRequirement}.");
                    }
                }
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                if (request.IsEquipped)
                {
                    var slot = request.SlotIndex ?? 0;
                    if (slot < 0 || slot > 2) throw new ArgumentException("Invalid slot index.");

                    // Tháo kỹ năng cũ đang nằm trong ô này ra
                    var others = await _repository.GetPlayerSkillsByPlayerId(actorPlayerProfileId);
                    foreach (var other in others)
                    {
                        if (other.PlayerSkillId != ps.PlayerSkillId && other.EquippedSlot.HasValue && other.EquippedSlot.Value == slot)
                        {
                            other.EquippedSlot = null;
                            // IsEquipped is derived from EquippedSlot; clear EquippedSlot only
                            await _repository.UpdatePlayerSkill(other);
                        }
                    }

                    // Đảm bảo kỹ năng này không bị gắn ở ô khác trùng lặp
                    foreach (var otherSameSkill in others)
                    {
                        if (otherSameSkill.PlayerSkillId != ps.PlayerSkillId && otherSameSkill.SkillId == ps.SkillId && otherSameSkill.EquippedSlot.HasValue)
                        {
                            otherSameSkill.EquippedSlot = null;
                            // IsEquipped is derived from EquippedSlot; clear EquippedSlot only
                            await _repository.UpdatePlayerSkill(otherSameSkill);
                        }
                    }

                    // Trang bị kỹ năng hiện tại
                    ps.EquippedSlot = slot;
                    // IsEquipped is derived from EquippedSlot
                }
                else
                {
                    // Hủy trang bị
                    ps.EquippedSlot = null;
                    // IsEquipped is derived from EquippedSlot
                }

                var updated = await _repository.UpdatePlayerSkill(ps);

                // =================================================================
                // QUAN TRỌNG NHẤT: Bắt buộc gọi SaveChangesAsync để tạo lệnh UPDATE 
                // xuống SQL Database TRƯỚC KHI gọi Commit Transaction!
                // =================================================================
                await _context.SaveChangesAsync(); 

                await tx.CommitAsync();

                var dto = _mapper.Map<PlayerSkillResponseDto>(updated);
                dto.CooldownSeconds = updated.Skill?.CooldownSeconds ?? 0;
                dto.BaseDamage = updated.Skill?.BaseDamage ?? 0.0;
                dto.EffectiveDamage = (updated.Skill?.BaseDamage ?? 0.0) * (1 + (updated.Skill?.DamageGrowthPercent ?? 0.0) / 100.0 * (updated.Level - 1))
                    + (updated.Skill?.DamagePerLevel ?? 0.0) * (updated.Level - 1);
                return dto;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<PlayerSkillResponseDto?> DismantlePlayerSkill(int actorPlayerProfileId, DismantlePlayerSkillRequestDto request)
        {
            var ps = await _repository.GetPlayerSkillById(request.PlayerSkillId)
                ?? throw new KeyNotFoundException("PlayerSkill not found.");

            if (ps.PlayerProfileId != actorPlayerProfileId)
                throw new UnauthorizedAccessException("PlayerSkill does not belong to actor.");

            // Do not allow dismantling an equipped skill
            if (ps.EquippedSlot.HasValue)
                throw new InvalidOperationException("Cannot dismantle an equipped skill. Please unequip it first.");

            // XP granted formula: base 100 * level
            int xpGranted = 100 * Math.Max(1, ps.Level);

            PlayerSkill? target = null;
            if (request.TargetPlayerSkillId.HasValue)
            {
                target = await _repository.GetPlayerSkillById(request.TargetPlayerSkillId.Value)
                    ?? throw new KeyNotFoundException("Target PlayerSkill not found.");

                if (target.PlayerProfileId != actorPlayerProfileId)
                    throw new UnauthorizedAccessException("Target PlayerSkill does not belong to actor.");

                // ensure target skill is compatible with player's class
                var playerProfile = await _context.PlayerProfiles.FindAsync(actorPlayerProfileId);
                var targetSkillDef = target.Skill ?? await _repository.GetSkillById(target.SkillId);
                if (playerProfile != null && targetSkillDef != null && !string.IsNullOrWhiteSpace(targetSkillDef.ClassRequirement))
                {
                    if (!targetSkillDef.ClassRequirement.Equals(playerProfile.Class, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Target skill is not usable by player's class.");
                }

                // Apply XP and handle level ups (simple threshold: 100 * currentLevel)
                target.Experience += xpGranted;
                while (target.Experience >= 100 * target.Level)
                {
                    target.Experience -= 100 * target.Level;
                    target.Level += 1;
                }

                await _repository.UpdatePlayerSkill(target);
            }

            // Remove the dismantled skill
            await _repository.DeletePlayerSkill(ps);

            // Return updated target DTO if present, else null
            if (target != null)
            {
                var dto = _mapper.Map<PlayerSkillResponseDto>(target);
                dto.CooldownSeconds = target.Skill?.CooldownSeconds ?? 0;
                dto.BaseDamage = target.Skill?.BaseDamage ?? 0.0;
                dto.EffectiveDamage = (target.Skill?.BaseDamage ?? 0.0) * (1 + (target.Skill?.DamageGrowthPercent ?? 0.0) / 100.0 * (target.Level - 1))
                    + (target.Skill?.DamagePerLevel ?? 0.0) * (target.Level - 1);
                return dto;
            }

            return null;
        }

        public async Task<PlayerSkillResponseDto> UnlockPlayerSkill(int actorPlayerProfileId, UnlockPlayerSkillRequestDto request)
        {
            var skill = await _repository.GetSkillById(request.SkillId)
                ?? throw new KeyNotFoundException("Skill not found.");

            var owned = await _repository.GetPlayerSkillsByPlayerId(actorPlayerProfileId);
            if (owned.Any(ps => ps.SkillId == request.SkillId))
                throw new InvalidOperationException("Player already owns this skill.");

            var newPlayerSkill = new PlayerSkill
            {
                PlayerProfileId = actorPlayerProfileId,
                SkillId = request.SkillId,
                Level = 1,
                Experience = 0,
                EquippedSlot = null,
                UnlockedAt = DateTime.UtcNow
            };

            var created = await _repository.CreatePlayerSkill(newPlayerSkill);
            var dto = _mapper.Map<PlayerSkillResponseDto>(created);
            dto.CooldownSeconds = skill.CooldownSeconds;
            dto.BaseDamage = skill.BaseDamage;
            dto.EffectiveDamage = skill.BaseDamage;
            dto.UnlockLevel = skill.UnlockLevel;
            return dto;
        }
    }
}
