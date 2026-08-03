using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace BLL.Services
{
    public class DungeonConfigService : IDungeonConfigService
    {
        private readonly IDungeonConfigRepository _repository;
        private readonly IMapper _mapper;

        public DungeonConfigService(IDungeonConfigRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DungeonConfigResponseDto?> GetDungeonById(int id)
        {
            var dungeon = await _repository.GetByIdWithChest(id);
            if (dungeon == null)
                return null;

            var dto = _mapper.Map<DungeonConfigResponseDto>(dungeon);
            if (dungeon.Chest != null)
            {
                dto.GoldMinReward = dungeon.Chest.GoldMinReward;
                dto.GoldMaxReward = dungeon.Chest.GoldMaxReward;
                dto.ExperienceReward = dungeon.Chest.ExperienceReward;
                if (dungeon.Chest.ChestItems != null)
                {
                    dto.PossibleDrops = _mapper.Map<List<ChestItemResponseDto>>(dungeon.Chest.ChestItems);
                }
            }
            return dto;
        }

        public async Task<DungeonConfigResponseDto> UpdateDungeon(int id, UpdateDungeonConfigRequestDto request)
        {
            var dungeon = await _repository.GetDungeonConfigById(id)
                ?? throw new KeyNotFoundException($"DungeonConfig with id {id} not found.");

            dungeon.Name = request.Name;
            dungeon.Description = request.Description;
            dungeon.Type = request.Type;
            dungeon.LevelRequirement = request.LevelRequirement;
            dungeon.MaxMembers = request.MaxMembers;
            dungeon.Difficulty = request.Difficulty;
            dungeon.RecommendedPower = request.RecommendedPower;
            dungeon.EnergyCost = request.EnergyCost;
            dungeon.ChestId = request.ChestId;
            dungeon.IsActive = request.IsActive;

            var updated = await _repository.UpdateDungeonConfig(dungeon);
            return _mapper.Map<DungeonConfigResponseDto>(updated);
        }

        public async Task<PagedResultDto<DungeonConfigResponseDto>> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetDungeonsPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);

            var dtos = _mapper.Map<List<DungeonConfigResponseDto>>(items);
            for (int i = 0; i < dtos.Count; i++)
            {
                var dungeon = items[i];
                if (dungeon.Chest != null)
                {
                    dtos[i].GoldMinReward = dungeon.Chest.GoldMinReward;
                    dtos[i].GoldMaxReward = dungeon.Chest.GoldMaxReward;
                    dtos[i].ExperienceReward = dungeon.Chest.ExperienceReward;
                    if (dungeon.Chest.ChestItems != null)
                    {
                        dtos[i].PossibleDrops = _mapper.Map<List<ChestItemResponseDto>>(dungeon.Chest.ChestItems);
                    }
                }
            }
            return new PagedResultDto<DungeonConfigResponseDto>(totalCount, dtos);
        }
    }
}
