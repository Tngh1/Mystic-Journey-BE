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
