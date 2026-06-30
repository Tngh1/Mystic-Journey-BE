using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;


namespace BLL.Services
{
    public class MonsterService : IMonsterService
    {
        private readonly IMonsterRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionManager _transactionManager;
        private readonly IDungeonConfigRepository _dungeonConfigRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public MonsterService(
            IMonsterRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IMapper mapper,
            ITransactionManager transactionManager,
            IDungeonConfigRepository dungeonConfigRepository,
            IInventoryRepository inventoryRepository)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _mapper = mapper;
            _transactionManager = transactionManager;
            _dungeonConfigRepository = dungeonConfigRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<MonsterDetailResponseDto?> GetMonsterById(int id)
        {
            var monster = await _repository.GetMonsterByIdWithDrops(id);
            if (monster == null)
                return null;

            var dto = _mapper.Map<MonsterDetailResponseDto>(monster);

            if (monster.MonsterDrops != null && monster.MonsterDrops.Any())
            {
                dto.MonsterDrops = _mapper.Map<List<MonsterDropResponseDto>>(monster.MonsterDrops.Where(d => d.IsActive));
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

            return _mapper.Map<MonsterDropResponseDto>(created);
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

            var dtos = _mapper.Map<List<MonsterDropResponseDto>>(items);

            return new PagedResultDto<MonsterDropResponseDto>(totalCount, dtos);
        }

        public async Task<MonsterDetailResponseDto?> GetMonsterForPlayer(int id, int playerProfileId)
        {
            var monster = await _repository.GetMonsterByIdWithDrops(id);
            if (monster == null)
                return null;

            var discovery = await _repository.GetPlayerDiscovery(playerProfileId, id);

            var dto = _mapper.Map<MonsterDetailResponseDto>(monster);

            // If player hasn't discovered this monster, mask details
            if (discovery == null || !discovery.IsDiscovered)
            {
                // Masked representation
                var masked = new MonsterDetailResponseDto
                {
                    MonsterId = monster.MonsterId,
                    Name = "?",
                    Type = "Unknown",
                    Description = string.Empty,
                    Level = 0,
                    MaxHp = 0,
                    Atk = 0,
                    Def = 0,
                    MoveSpeed = 0,
                    AttackSpeed = 0,
                    CritRate = 0,
                    CritDamage = 0,
                    ExperienceReward = 0,
                    GoldReward = 0,
                    ImageUrl = null,
                    IsActive = monster.IsActive,
                    MonsterDrops = new List<MonsterDropResponseDto>()
                };

                return masked;
            }

            // Otherwise return full detail including drops
            if (monster.MonsterDrops != null && monster.MonsterDrops.Any())
            {
                dto.MonsterDrops = _mapper.Map<List<MonsterDropResponseDto>>(monster.MonsterDrops.Where(d => d.IsActive));
            }

            return dto;
        }

        public async Task<PagedResultDto<PlayerMonsterCatalogItemDto>> GetMonsterCatalogForPlayer(
            int playerProfileId, int page, int pageSize, string? search, string? type)
        {
            var (totalCount, monsters) = await _repository.GetMonstersPaged(page, pageSize, search, type, true);
            var discoveredIds = await _repository.GetDiscoveredMonsterIds(playerProfileId);
            var discoveries = await _repository.GetPlayerDiscoveriesDict(playerProfileId);

            var items = monsters.Select(m =>
            {
                var isDiscovered = discoveredIds.Contains(m.MonsterId);
                discoveries.TryGetValue(m.MonsterId, out var discovery);

                if (!isDiscovered)
                {
                    return new PlayerMonsterCatalogItemDto
                    {
                        MonsterId = m.MonsterId,
                        Name = "?",
                        Type = "Unknown",
                        Description = string.Empty,
                        Level = 0,
                        MaxHp = 0,
                        Atk = 0,
                        Def = 0,
                        ExperienceReward = 0,
                        GoldReward = 0,
                        ImageUrl = null,
                        IsDiscovered = false,
                        TimesDefeated = discovery?.TimesDefeated ?? 0
                    };
                }

                return new PlayerMonsterCatalogItemDto
                {
                    MonsterId = m.MonsterId,
                    Name = m.Name,
                    Type = m.Type,
                    Description = m.Description,
                    Level = m.Level,
                    MaxHp = m.MaxHp,
                    Atk = m.Atk,
                    Def = m.Def,
                    ExperienceReward = m.ExperienceReward,
                    GoldReward = m.GoldReward,
                    ImageUrl = m.ImageUrl,
                    IsDiscovered = true,
                    TimesDefeated = discovery?.TimesDefeated ?? 0
                };
            }).ToList();

            return new PagedResultDto<PlayerMonsterCatalogItemDto>(totalCount, items);
        }

        public async Task<List<MonsterSpawnResponseDto>> GetSpawnsForPlayer(
            int playerProfileId, string mapName, string? regionName, int? dungeonId)
        {
            var normalizedMap = NormalizeMapName(mapName);
            var spawns = await _repository.GetActiveSpawns(normalizedMap, regionName, dungeonId);
            var suppressedBossIds = await _repository.GetCompletedQuestBossMonsterIds(playerProfileId);

            return spawns
                .Where(s => s.Monster != null && !suppressedBossIds.Contains(s.MonsterId))
                .Select(s => _mapper.Map<MonsterSpawnResponseDto>(s))
                .ToList();
        }

        public async Task<MonsterSpawnResponseDto> CreateSpawn(CreateMonsterSpawnRequestDto request)
        {
            var monster = await _repository.GetMonsterById(request.MonsterId)
                ?? throw new KeyNotFoundException($"Monster with id {request.MonsterId} not found.");

            if (request.DungeonId.HasValue)
            {
                var dungeonExists = await _dungeonConfigRepository.DungeonExists(request.DungeonId.Value);
                if (!dungeonExists)
                    throw new KeyNotFoundException($"Dungeon with id {request.DungeonId.Value} not found.");
            }

            var spawn = new MonsterSpawn
            {
                MonsterId = request.MonsterId,
                MapName = NormalizeMapName(request.MapName),
                RegionName = request.RegionName,
                Location = request.Location,
                SpawnCount = request.SpawnCount,
                RespawnSeconds = request.RespawnSeconds,
                DungeonId = request.DungeonId,
                IsActive = request.IsActive
            };

            var created = await _repository.CreateSpawn(spawn);
            created.Monster = monster;
            return _mapper.Map<MonsterSpawnResponseDto>(created);
        }

        public async Task<List<MonsterSpawnResponseDto>> GetSpawnsByMonsterId(int monsterId)
        {
            var spawns = await _repository.GetSpawnsByMonsterId(monsterId);
            var monster = await _repository.GetMonsterById(monsterId);

            return spawns.Select(s =>
            {
                s.Monster ??= monster;
                return _mapper.Map<MonsterSpawnResponseDto>(s);
            }).ToList();
        }

        public async Task<PlayerMonsterCatalogItemDto> DiscoverMonster(int playerProfileId, int monsterId)
        {
            var monster = await _repository.GetMonsterById(monsterId)
                ?? throw new KeyNotFoundException($"Monster with id {monsterId} not found.");

            var existing = await _repository.GetPlayerDiscovery(playerProfileId, monsterId);
            var wasDiscovered = existing?.IsDiscovered ?? false;

            await _repository.CreateOrUpdatePlayerDiscovery(new PlayerMonsterDiscovery
            {
                PlayerProfileId = playerProfileId,
                MonsterId = monsterId,
                IsDiscovered = true,
                DiscoveredAt = DateTime.UtcNow,
                TimesDefeated = existing?.TimesDefeated ?? 0
            });

            return new PlayerMonsterCatalogItemDto
            {
                MonsterId = monster.MonsterId,
                Name = monster.Name,
                Type = monster.Type,
                Description = monster.Description,
                Level = monster.Level,
                MaxHp = monster.MaxHp,
                Atk = monster.Atk,
                Def = monster.Def,
                ExperienceReward = monster.ExperienceReward,
                GoldReward = monster.GoldReward,
                ImageUrl = monster.ImageUrl,
                IsDiscovered = true,
                TimesDefeated = existing?.TimesDefeated ?? 0
            };
        }

        public async Task<MonsterDefeatResponseDto> DefeatMonster(
            int playerProfileId, int monsterId, MonsterDefeatRequestDto? request)
        {
            var monster = await _repository.GetMonsterByIdWithDrops(monsterId)
                ?? throw new KeyNotFoundException($"Monster with id {monsterId} not found.");

            if (!monster.IsActive)
                throw new InvalidOperationException($"Monster {monsterId} is not active.");

            var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            var existingDiscovery = await _repository.GetPlayerDiscovery(playerProfileId, monsterId);
            var wasDiscovered = existingDiscovery?.IsDiscovered ?? false;

            var drops = await _repository.GetActiveDropsByMonsterId(monsterId);
            var rolledItems = RollDrops(drops);

            await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                profile.ExperiencePoints += monster.ExperienceReward;
                profile.Gold += monster.GoldReward;
                profile.UpdatedAt = DateTime.UtcNow;

                foreach (var drop in rolledItems)
                    await AddItemToInventory(playerProfileId, drop.ItemId, drop.Quantity);

                await _repository.CreateOrUpdatePlayerDiscovery(new PlayerMonsterDiscovery
                {
                    PlayerProfileId = playerProfileId,
                    MonsterId = monsterId,
                    IsDiscovered = true,
                    DiscoveredAt = existingDiscovery?.DiscoveredAt ?? DateTime.UtcNow,
                    TimesDefeated = (existingDiscovery?.TimesDefeated ?? 0) + 1
                });

                await _playerProfileRepository.UpdatePlayerProfile(profile);
            });

            return new MonsterDefeatResponseDto
            {
                MonsterId = monster.MonsterId,
                MonsterName = monster.Name,
                WasDiscovered = wasDiscovered,
                ExperienceEarned = monster.ExperienceReward,
                GoldEarned = monster.GoldReward,
                PlayerLevel = profile.Level,
                PlayerExperience = profile.ExperiencePoints,
                PlayerGold = profile.Gold,
                DroppedItems = rolledItems
            };
        }



        private static List<MonsterDroppedItemDto> RollDrops(IEnumerable<MonsterDrop> drops)
        {
            var result = new List<MonsterDroppedItemDto>();

            foreach (var drop in drops)
            {
                if (drop.Item == null)
                    continue;

                var shouldDrop = drop.IsGuaranteed || Random.Shared.NextDouble() * 100 <= drop.DropRate;
                if (!shouldDrop)
                    continue;

                var quantity = drop.MaxQuantity > drop.MinQuantity
                    ? Random.Shared.Next(drop.MinQuantity, drop.MaxQuantity + 1)
                    : drop.MinQuantity;

                result.Add(new MonsterDroppedItemDto
                {
                    ItemId = drop.ItemId,
                    ItemName = drop.Item.Name,
                    ItemIconUrl = drop.Item.IconUrl,
                    Quantity = quantity
                });
            }

            return result;
        }

        private async Task AddItemToInventory(int playerProfileId, int itemId, int quantity)
        {
            var existing = await _inventoryRepository.GetByPlayerAndItem(playerProfileId, itemId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                await _inventoryRepository.UpdateItem(existing);
            }
            else
            {
                await _inventoryRepository.AddItem(new InventoryItem
                {
                    PlayerProfileId = playerProfileId,
                    ItemId = itemId,
                    Quantity = quantity,
                    IsEquipped = false,
                    IsSkin = false,
                    EnhancementLevel = 0
                });
            }
        }

        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
                return "ElfForest";

            var normalized = mapName.Trim();
            return string.Equals(normalized, "ElfLand", StringComparison.OrdinalIgnoreCase)
                ? "ElfForest"
                : normalized;
        }
    }
}
