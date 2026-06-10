using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;


namespace BLL.Services
{
    public class MonsterService : IMonsterService
    {
        private readonly IMonsterRepository _repository;
        private readonly IMapper _mapper;

        public MonsterService(IMonsterRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MonsterDetailResponseDto?> GetMonsterById(int id)
        {
            var monster = await _repository.GetMonsterByIdWithDrops(id);
            if (monster == null)
                return null;

            var dto = _mapper.Map<MonsterDetailResponseDto>(monster);

            if (monster.MonsterDrops != null && monster.MonsterDrops.Any())
            {
                dto.MonsterDrops = monster.MonsterDrops
                    .Where(d => d.IsActive)
                    .Select(d => new MonsterDropResponseDto
                    {
                        MonsterDropId = d.MonsterDropId,
                        MonsterId = d.MonsterId,
                        ItemId = d.ItemId,
                        ItemName = d.Item?.Name,
                        DropRate = d.DropRate,
                        MinQuantity = d.MinQuantity,
                        MaxQuantity = d.MaxQuantity,
                        IsGuaranteed = d.IsGuaranteed,
                        IsActive = d.IsActive
                    })
                    .ToList();
            }

            return dto;
        }

        public async Task<MonsterResponseDto> CreateMonster(CreateMonsterRequestDto request)
        {
            var monster = _mapper.Map<Monster>(request);
            monster.CreatedAt = DateTime.UtcNow;

            var created = await _repository.CreateMonster(monster);
            return _mapper.Map<MonsterResponseDto>(created);
        }

        public async Task<MonsterResponseDto> UpdateMonster(int id, UpdateMonsterRequestDto request)
        {
            var monster = await _repository.GetMonsterById(id)
                ?? throw new KeyNotFoundException($"Monster with id {id} not found.");

            monster.Name = request.Name;
            monster.Type = request.Type;
            monster.Description = request.Description;
            monster.Level = request.Level;
            monster.MaxHp = request.MaxHp;
            monster.Atk = request.Atk;
            monster.Def = request.Def;
            monster.MoveSpeed = request.MoveSpeed;
            monster.AttackSpeed = request.AttackSpeed;
            monster.CritRate = request.CritRate;
            monster.CritDamage = request.CritDamage;
            monster.ExperienceReward = request.ExperienceReward;
            monster.GoldReward = request.GoldReward;
            monster.ImageUrl = request.ImageUrl;
            monster.IsActive = request.IsActive;

            var updated = await _repository.UpdateMonster(monster);
            return _mapper.Map<MonsterResponseDto>(updated);
        }

        public async Task<MonsterDropResponseDto> AddMonsterDrop(int monsterId, CreateMonsterDropRequestDto request)
        {
            var monster = await _repository.GetMonsterById(monsterId)
                ?? throw new KeyNotFoundException($"Monster with id {monsterId} not found.");

            var drop = new MonsterDrop
            {
                MonsterId = monsterId,
                ItemId = request.ItemId,
                DropRate = request.DropRate,
                MinQuantity = request.MinQuantity,
                MaxQuantity = request.MaxQuantity,
                IsGuaranteed = request.IsGuaranteed,
                IsActive = request.IsActive
            };

            var created = await _repository.CreateDrop(drop);

            return new MonsterDropResponseDto
            {
                MonsterDropId = created.MonsterDropId,
                MonsterId = created.MonsterId,
                ItemId = created.ItemId,
                DropRate = created.DropRate,
                MinQuantity = created.MinQuantity,
                MaxQuantity = created.MaxQuantity,
                IsGuaranteed = created.IsGuaranteed,
                IsActive = created.IsActive
            };
        }

        public async Task<PagedResultDto<MonsterResponseDto>> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var (totalCount, items) = await _repository.GetMonstersPaged(page, pageSize, search, type, isActive);
            var dtos = items.Select(m => _mapper.Map<MonsterResponseDto>(m)).ToList();
            return new PagedResultDto<MonsterResponseDto>(totalCount, dtos);
        }

        public async Task<PagedResultDto<MonsterDropResponseDto>> GetMonsterDropsPaged(int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetMonsterDropsPaged(page, pageSize);

            var dtos = items.Select(d => new MonsterDropResponseDto
            {
                MonsterDropId = d.MonsterDropId,
                MonsterId = d.MonsterId,
                ItemId = d.ItemId,
                ItemName = d.Item?.Name,
                DropRate = d.DropRate,
                MinQuantity = d.MinQuantity,
                MaxQuantity = d.MaxQuantity,
                IsGuaranteed = d.IsGuaranteed,
                IsActive = d.IsActive
            }).ToList();

            return new PagedResultDto<MonsterDropResponseDto>(totalCount, dtos);
        }
    }
}
