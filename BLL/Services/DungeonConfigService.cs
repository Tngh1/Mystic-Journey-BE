using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
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
            var dungeon = await _repository.GetDungeonConfigById(id);
            if (dungeon == null)
                return null;

            return _mapper.Map<DungeonConfigResponseDto>(dungeon);
        }

        public async Task<DungeonConfigResponseDto> CreateDungeon(CreateDungeonConfigRequestDto request)
        {
            var dungeon = _mapper.Map<DungeonConfig>(request);

            var created = await _repository.CreateDungeonConfig(dungeon);
            return _mapper.Map<DungeonConfigResponseDto>(created);
        }

        public async Task<DungeonConfigResponseDto> UpdateDungeon(int id, UpdateDungeonConfigRequestDto request)
        {
            var dungeon = await _repository.GetDungeonConfigById(id)
                ?? throw new KeyNotFoundException($"DungeonConfig with id {id} not found.");

            dungeon.Name = request.Name;
            dungeon.Description = request.Description;
            dungeon.ImageUrl = request.ImageUrl;
            dungeon.Type = request.Type;
            dungeon.LevelRequirement = request.LevelRequirement;
            dungeon.MaxMembers = request.MaxMembers;
            dungeon.Difficulty = request.Difficulty;
            dungeon.RecommendedPower = request.RecommendedPower;
            dungeon.ChestId = request.ChestId;
            dungeon.IsActive = request.IsActive;

            var updated = await _repository.UpdateDungeonConfig(dungeon);
            return _mapper.Map<DungeonConfigResponseDto>(updated);
        }

        public IQueryable<DungeonConfigResponseDto> GetDungeonsQueryable()
        {
            return _repository.GetDungeonConfigsQueryable()
                .ProjectTo<DungeonConfigResponseDto>(_mapper.ConfigurationProvider);
        }
    }
}
