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
        private readonly IChestRepository _chestRepository;
        private readonly IMapper _mapper;

        public DungeonConfigService(IDungeonConfigRepository repository, IChestRepository chestRepository, IMapper mapper)
        {
            _repository = repository;
            _chestRepository = chestRepository;
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

        public async Task<ChestItemResponseDto> AddChestItem(int dungeonId, CreateChestItemRequestDto request)
        {
            var dungeon = await _repository.GetByIdWithChest(dungeonId);
            if (dungeon == null) throw new KeyNotFoundException("Dungeon not found.");

            if (dungeon.ChestId == null)
            {
                var newChest = new Chest { Name = dungeon.Name + " Chest" };
                await _chestRepository.CreateChest(newChest);
                dungeon.ChestId = newChest.ChestId;
                await _repository.UpdateDungeonConfig(dungeon);
            }

            var chestItem = new ChestItem
            {
                ChestId = dungeon.ChestId.Value,
                ItemId = request.ItemId,
                QuantityMin = request.QuantityMin,
                QuantityMax = request.QuantityMax,
                DropRate = request.DropRate,
                IsGuaranteed = request.IsGuaranteed
            };

            await _chestRepository.AddChestItem(chestItem);
            var savedItem = await _chestRepository.GetChestItemById(chestItem.ChestItemId);
            return _mapper.Map<ChestItemResponseDto>(savedItem);
        }

        public async Task<ChestItemResponseDto> UpdateChestItem(int dungeonId, int chestItemId, CreateChestItemRequestDto request)
        {
            var chestItem = await _chestRepository.GetChestItemById(chestItemId);
            if (chestItem == null) throw new KeyNotFoundException("ChestItem not found.");

            chestItem.QuantityMin = request.QuantityMin;
            chestItem.QuantityMax = request.QuantityMax;
            chestItem.DropRate = request.DropRate;
            chestItem.IsGuaranteed = request.IsGuaranteed;

            await _chestRepository.UpdateChestItem(chestItem);
            return _mapper.Map<ChestItemResponseDto>(chestItem);
        }

        public async Task RemoveChestItem(int dungeonId, int chestItemId)
        {
            await _chestRepository.RemoveChestItem(chestItemId);
        }
    }
}
