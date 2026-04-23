using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;
        private readonly IPlayerSkillRepository _playerSkillRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public SkillService(
            ISkillRepository skillRepository,
            IPlayerSkillRepository playerSkillRepository,
            IPlayerProfileRepository profileRepository,
            IMapper mapper)
        {
            _skillRepository = skillRepository;
            _playerSkillRepository = playerSkillRepository;
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<SkillListResponseDto> GetAllSkillsAsync()
        {
            var skills = await _skillRepository.GetAllActiveAsync();

            return new SkillListResponseDto
            {
                Success = true,
                Message = "Skills retrieved successfully.",
                Skills = skills.Select(_mapper.Map<SkillResponseDto>).ToList(),
                TotalCount = skills.Count
            };
        }

        public async Task<SkillListResponseDto> GetSkillsByClassAsync(PlayerProfile.CharacterClass characterClass)
        {
            var skills = await _skillRepository.GetByClassAsync(characterClass);

            return new SkillListResponseDto
            {
                Success = true,
                Message = $"Skills for {characterClass} retrieved successfully.",
                Skills = skills.Select(_mapper.Map<SkillResponseDto>).ToList(),
                TotalCount = skills.Count
            };
        }

        public async Task<SkillListResponseDto> GetAvailableSkillsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new SkillListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var skills = await _skillRepository.GetAvailableForLevelAsync(profile.Level, profile.Class);

            return new SkillListResponseDto
            {
                Success = true,
                Message = "Available skills retrieved successfully.",
                Skills = skills.Select(_mapper.Map<SkillResponseDto>).ToList(),
                TotalCount = skills.Count
            };
        }

        public async Task<SkillApiResponseDto> GetSkillByIdAsync(Guid skillId)
        {
            var skill = await _skillRepository.GetByIdAsync(skillId);

            if (skill == null)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Skill not found."
                };
            }

            return new SkillApiResponseDto
            {
                Success = true,
                Message = "Skill retrieved successfully.",
                Skill = _mapper.Map<SkillResponseDto>(skill)
            };
        }

        public async Task<SkillListResponseDto> GetPlayerSkillsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new SkillListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerSkills = await _playerSkillRepository.GetByPlayerProfileIdAsync(profile.Id);

            var dtos = playerSkills.Select(ps => new PlayerSkillResponseDto
            {
                PlayerSkillId = ps.Id,
                PlayerProfileId = ps.PlayerProfileId,
                SkillId = ps.SkillId,
                SkillName = ps.Skill?.Name ?? string.Empty,
                SkillDescription = ps.Skill?.Description,
                Category = ps.Skill?.Type.ToString() ?? string.Empty,
                DamageType = ps.Skill?.DamageType.ToString() ?? string.Empty,
                TargetType = ps.Skill?.TargetType.ToString() ?? string.Empty,
                Level = ps.Level,
                Experience = ps.Experience,
                IsEquipped = ps.IsEquipped,
                ManaCost = ps.Skill?.ManaCost ?? 0,
                CooldownSeconds = ps.Skill?.CooldownSeconds ?? 0,
                BaseDamage = ps.Skill?.BaseDamage ?? 0,
                UnlockedAt = ps.UnlockedAt
            }).ToList();

            return new SkillListResponseDto
            {
                Success = true,
                Message = "Player skills retrieved successfully.",
                PlayerSkills = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<SkillListResponseDto> GetEquippedSkillsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new SkillListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var equippedSkills = await _playerSkillRepository.GetEquippedSkillsAsync(profile.Id);

            var dtos = equippedSkills.Select(ps => new PlayerSkillResponseDto
            {
                PlayerSkillId = ps.Id,
                PlayerProfileId = ps.PlayerProfileId,
                SkillId = ps.SkillId,
                SkillName = ps.Skill?.Name ?? string.Empty,
                SkillDescription = ps.Skill?.Description,
                Category = ps.Skill?.Type.ToString() ?? string.Empty,
                DamageType = ps.Skill?.DamageType.ToString() ?? string.Empty,
                TargetType = ps.Skill?.TargetType.ToString() ?? string.Empty,
                Level = ps.Level,
                Experience = ps.Experience,
                IsEquipped = ps.IsEquipped,
                ManaCost = ps.Skill?.ManaCost ?? 0,
                CooldownSeconds = ps.Skill?.CooldownSeconds ?? 0,
                BaseDamage = ps.Skill?.BaseDamage ?? 0,
                UnlockedAt = ps.UnlockedAt
            }).ToList();

            return new SkillListResponseDto
            {
                Success = true,
                Message = "Equipped skills retrieved successfully.",
                PlayerSkills = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<SkillApiResponseDto> UnlockSkillAsync(Guid accountId, UnlockSkillRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var skill = await _skillRepository.GetByIdAsync(request.SkillId);
            if (skill == null)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Skill not found."
                };
            }

            if (await _playerSkillRepository.HasSkillAsync(profile.Id, request.SkillId))
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "You already have this skill."
                };
            }

            if (profile.Level < skill.UnlockLevel)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = $"You need to be level {skill.UnlockLevel} to unlock this skill."
                };
            }

            if (skill.ClassRequirement != profile.Class && skill.ClassRequirement != PlayerProfile.CharacterClass.Knight)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = $"This skill is only available for {skill.ClassRequirement} class."
                };
            }

            var playerSkill = new PlayerSkill
            {
                Id = Guid.NewGuid(),
                PlayerProfileId = profile.Id,
                SkillId = skill.Id,
                Level = 1,
                Experience = 0,
                IsEquipped = false,
                UnlockedAt = DateTime.UtcNow
            };

            await _playerSkillRepository.CreateAsync(playerSkill);

            var response = new PlayerSkillResponseDto
            {
                PlayerSkillId = playerSkill.Id,
                PlayerProfileId = playerSkill.PlayerProfileId,
                SkillId = playerSkill.SkillId,
                SkillName = skill.Name,
                SkillDescription = skill.Description,
                Category = skill.Type.ToString(),
                DamageType = skill.DamageType.ToString(),
                TargetType = skill.TargetType.ToString(),
                Level = playerSkill.Level,
                Experience = playerSkill.Experience,
                IsEquipped = playerSkill.IsEquipped,
                ManaCost = skill.ManaCost,
                CooldownSeconds = skill.CooldownSeconds,
                BaseDamage = skill.BaseDamage,
                UnlockedAt = playerSkill.UnlockedAt
            };

            return new SkillApiResponseDto
            {
                Success = true,
                Message = $"Skill '{skill.Name}' unlocked successfully!"
            };
        }

        public async Task<SkillApiResponseDto> UpgradeSkillAsync(Guid accountId, UpgradeSkillRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerSkill = await _playerSkillRepository.GetByIdWithDetailsAsync(request.PlayerSkillId);
            if (playerSkill == null || playerSkill.PlayerProfileId != profile.Id)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Player skill not found."
                };
            }

            var stats = await _profileRepository.GetStatsByProfileIdAsync(profile.Id);
            if (stats == null || stats.SkillPoints < 1)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Not enough skill points."
                };
            }

            if (playerSkill.Level >= 20)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Skill has reached maximum level."
                };
            }

            var expRequired = playerSkill.Level * 100;
            playerSkill.Experience += expRequired;
            stats.SkillPoints -= 1;

            while (playerSkill.Experience >= expRequired && playerSkill.Level < 20)
            {
                playerSkill.Experience -= expRequired;
                playerSkill.Level++;
            }

            await _playerSkillRepository.UpdateAsync(playerSkill);
            await _profileRepository.UpdateStatsAsync(stats);

            return new SkillApiResponseDto
            {
                Success = true,
                Message = $"Skill upgraded to level {playerSkill.Level}!"
            };
        }

        public async Task<SkillApiResponseDto> EquipSkillAsync(Guid accountId, EquipSkillRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerSkill = await _playerSkillRepository.GetByIdWithDetailsAsync(request.PlayerSkillId);
            if (playerSkill == null || playerSkill.PlayerProfileId != profile.Id)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Player skill not found."
                };
            }

            if (playerSkill.IsEquipped)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Skill is already equipped."
                };
            }

            playerSkill.IsEquipped = true;
            await _playerSkillRepository.UpdateAsync(playerSkill);

            return new SkillApiResponseDto
            {
                Success = true,
                Message = $"Skill '{playerSkill.Skill?.Name}' equipped successfully!"
            };
        }

        public async Task<SkillApiResponseDto> UnequipSkillAsync(Guid accountId, Guid playerSkillId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var playerSkill = await _playerSkillRepository.GetByIdAsync(playerSkillId);
            if (playerSkill == null || playerSkill.PlayerProfileId != profile.Id)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Player skill not found."
                };
            }

            if (!playerSkill.IsEquipped)
            {
                return new SkillApiResponseDto
                {
                    Success = false,
                    Message = "Skill is not equipped."
                };
            }

            playerSkill.IsEquipped = false;
            await _playerSkillRepository.UpdateAsync(playerSkill);

            return new SkillApiResponseDto
            {
                Success = true,
                Message = "Skill unequipped successfully."
            };
        }
    }
}
