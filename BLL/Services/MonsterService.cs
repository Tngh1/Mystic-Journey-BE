using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System.Linq;


namespace BLL.Services
{
    // Executes core business logic for i monster service.
    public class MonsterService : IMonsterService
    {
        private readonly IMonsterRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionManager _transactionManager;
        private readonly IDungeonConfigRepository _dungeonConfigRepository;
        private readonly IInventoryRepository _inventoryRepository;

        private readonly IRewardDeliveryService _rewardDeliveryService;
        // Initialize this instance from repository, player profile repository, mapper, and transaction manager and store repository, player profile repository, mapper, transaction manager, and dungeon config repository for later operations.
        public MonsterService(
            IMonsterRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IMapper mapper,
            ITransactionManager transactionManager,
            IDungeonConfigRepository dungeonConfigRepository,
            IInventoryRepository inventoryRepository,
            IRewardDeliveryService rewardDeliveryService)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _mapper = mapper;
            _transactionManager = transactionManager;
            _dungeonConfigRepository = dungeonConfigRepository;
            _inventoryRepository = inventoryRepository;
            _rewardDeliveryService = rewardDeliveryService;
        }

        // Executes core business logic for get monster by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed MonsterDetailResponseDto? result asynchronously.
        public async Task<MonsterDetailResponseDto?> GetMonsterById(int id)
        {
            var monster = await _repository.GetMonsterByIdWithDrops(id);
            if (monster == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            var dto = _mapper.Map<MonsterDetailResponseDto>(monster);  // Transform domain entity into DTO for the API response layer

            if (monster.MonsterDrops != null && monster.MonsterDrops.Any())
            {
                dto.MonsterDrops = _mapper.Map<List<MonsterDropResponseDto>>(monster.MonsterDrops.Where(d => d.IsActive));  // Filter records matching the predicate
            }

            return dto;
        }

        // Executes core business logic for update monster.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MonsterResponseDto result asynchronously.
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
            return _mapper.Map<MonsterResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for add monster drop.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MonsterDropResponseDto result asynchronously.
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

            return _mapper.Map<MonsterDropResponseDto>(created);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get monsters paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<MonsterResponseDto result asynchronously.
        public async Task<PagedResultDto<MonsterResponseDto>> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetMonstersPaged(page, pageSize, search, type, isActive, sortBy, sortOrder);
            var dtos = items.Select(m => _mapper.Map<MonsterResponseDto>(m)).ToList();  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<MonsterResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get monster drops paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<MonsterDropResponseDto result asynchronously.
        public async Task<PagedResultDto<MonsterDropResponseDto>> GetMonsterDropsPaged(int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetMonsterDropsPaged(page, pageSize);

            var dtos = _mapper.Map<List<MonsterDropResponseDto>>(items);  // Transform domain entity into DTO for the API response layer

            return new PagedResultDto<MonsterDropResponseDto>(totalCount, dtos);
        }

        // Executes core business logic for get monster for player.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed MonsterDetailResponseDto? result asynchronously.
        public async Task<MonsterDetailResponseDto?> GetMonsterForPlayer(int id, int playerProfileId)
        {
            var monster = await _repository.GetMonsterByIdWithDrops(id);
            if (monster == null)  // Entity not found — short-circuit with appropriate error result
                return null;

            var discovery = await _repository.GetPlayerDiscovery(playerProfileId, id);

            var dto = _mapper.Map<MonsterDetailResponseDto>(monster);  // Transform domain entity into DTO for the API response layer

            if (discovery == null || !discovery.IsDiscovered)
            {
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

            if (monster.MonsterDrops != null && monster.MonsterDrops.Any())
            {
                dto.MonsterDrops = _mapper.Map<List<MonsterDropResponseDto>>(monster.MonsterDrops.Where(d => d.IsActive));  // Filter records matching the predicate
            }

            return dto;
        }

        // Load monster catalog for player using player profile id, page, page size, and search; it loads monsters paged, loads discovered monster ids, loads player discoveries dict, projects records into the output shape, and materializes the query results and guards invalid or unavailable states.
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

        // Load spawns for player using player profile id, map name, region name, and dungeon id; it loads active spawns, loads completed quest boss monster ids, filters the eligible records, projects records into the output shape, and builds map.
        public async Task<List<MonsterSpawnResponseDto>> GetSpawnsForPlayer(
            int playerProfileId, string mapName, string? regionName, int? dungeonId)
        {
            var normalizedMap = NormalizeMapName(mapName);
            var spawns = await _repository.GetActiveSpawns(normalizedMap, regionName, dungeonId);

            var suppressedBossIds = dungeonId.HasValue
                ? new HashSet<int>()
                : await _repository.GetCompletedQuestBossMonsterIds(playerProfileId);

            return spawns
                .Where(s => s.Monster != null && !suppressedBossIds.Contains(s.MonsterId))  // Filter records matching the predicate
                .Select(s => _mapper.Map<MonsterSpawnResponseDto>(s))  // Transform domain entity into DTO for the API response layer
                .ToList();
        }

        // Executes core business logic for create spawn.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MonsterSpawnResponseDto result asynchronously.
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

            var fullSpawn = await _repository.GetSpawnById(created.MonsterSpawnId) ?? created;
            return _mapper.Map<MonsterSpawnResponseDto>(fullSpawn);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for update spawn.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MonsterSpawnResponseDto result asynchronously.
        public async Task<MonsterSpawnResponseDto> UpdateSpawn(int spawnId, UpdateMonsterSpawnRequestDto request)
        {
            var spawn = await _repository.GetSpawnById(spawnId);
            if (spawn == null)  // Entity not found — short-circuit with appropriate error result
            {
                throw new KeyNotFoundException($"Spawn with id {spawnId} not found.");
            }

            spawn.SpawnCount = request.SpawnCount;
            spawn.RespawnSeconds = request.RespawnSeconds;

            var updated = await _repository.UpdateSpawn(spawn);
            return _mapper.Map<MonsterSpawnResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for delete spawn.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        public async Task DeleteSpawn(int spawnId)
        {
            var spawn = await _repository.GetSpawnById(spawnId);
            if (spawn == null)  // Entity not found — short-circuit with appropriate error result
            {
                throw new KeyNotFoundException($"Spawn with id {spawnId} not found.");
            }
            await _repository.DeleteSpawn(spawnId);
        }

        // Executes core business logic for get spawns by dungeon id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed List<MonsterSpawnResponseDto result asynchronously.
        public async Task<List<MonsterSpawnResponseDto>> GetSpawnsByDungeonId(int dungeonId)
        {
            var spawns = await _repository.GetActiveSpawns(string.Empty, null, dungeonId);
            return _mapper.Map<List<MonsterSpawnResponseDto>>(spawns);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get spawns by monster id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed List<MonsterSpawnResponseDto result asynchronously.
        public async Task<List<MonsterSpawnResponseDto>> GetSpawnsByMonsterId(int monsterId)
        {
            var spawns = await _repository.GetSpawnsByMonsterId(monsterId);
            var monster = await _repository.GetMonsterById(monsterId);

            return spawns.Select(s =>
            {
                s.Monster ??= monster;
                return _mapper.Map<MonsterSpawnResponseDto>(s);  // Transform domain entity into DTO for the API response layer
            }).ToList();
        }

        // Executes core business logic for discover monster.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed PlayerMonsterCatalogItemDto result asynchronously.
        public async Task<PlayerMonsterCatalogItemDto> DiscoverMonster(int playerProfileId, int monsterId)
        {
            var monster = await _repository.GetMonsterById(monsterId)
                ?? throw new KeyNotFoundException($"Monster with id {monsterId} not found.");

            var existing = await _repository.GetPlayerDiscovery(playerProfileId, monsterId);

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

        // Process defeat monster using player profile id, monster id, and request; it loads monster by id with drops, loads player profile by id, loads player discovery, filters the eligible records, and creates experience and guards invalid or unavailable states, keeps dependent writes atomic, and processes each matching entry.
        public async Task<MonsterDefeatResponseDto> DefeatMonster(
            int playerProfileId, int monsterId, MonsterDefeatRequestDto? request)
        {
            var monster = await _repository.GetMonsterByIdWithDrops(monsterId)
                ?? throw new KeyNotFoundException($"Monster with id {monsterId} not found.");

            if (!monster.IsActive)
                throw new InvalidOperationException($"Monster {monsterId} is not active.");  // Unexpected runtime state — propagate to global error handler

            var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId)
                ?? throw new KeyNotFoundException($"PlayerProfile {playerProfileId} not found.");

            var existingDiscovery = await _repository.GetPlayerDiscovery(playerProfileId, monsterId);
            var wasDiscovered = existingDiscovery?.IsDiscovered ?? false;

            var expEarned = monster.ExperienceReward;
            var goldEarned = monster.GoldReward;

            var rolledItems = RollDrops(monster.MonsterDrops.Where(d => d.IsActive));  // Filter records matching the predicate

            await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                profile.AddExperience(expEarned);
                profile.Gold += goldEarned;
                await _playerProfileRepository.UpdatePlayerProfile(profile);

                foreach (var item in rolledItems)
                    await AddItemToInventory(playerProfileId, item.ItemId, item.Quantity);

                await _repository.CreateOrUpdatePlayerDiscovery(new PlayerMonsterDiscovery
                {
                    PlayerProfileId = playerProfileId,
                    MonsterId = monsterId,
                    IsDiscovered = true,
                    DiscoveredAt = existingDiscovery?.DiscoveredAt ?? DateTime.UtcNow,
                    TimesDefeated = (existingDiscovery?.TimesDefeated ?? 0) + 1
                });
            });

            return new MonsterDefeatResponseDto
            {
                MonsterId = monster.MonsterId,
                MonsterName = monster.Name,
                WasDiscovered = wasDiscovered,
                ExperienceEarned = expEarned,
                GoldEarned = goldEarned,
                PlayerLevel = profile.Level,
                PlayerExperience = profile.ExperiencePoints,
                PlayerGold = profile.Gold,
                DroppedItems = rolledItems
            };
        }



        // Executes core business logic for roll drops.
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

        // Executes core business logic for add item to inventory.
        // Logic details: validates required non-empty string arguments.
        // Completes asynchronously upon successful execution.
        private async Task AddItemToInventory(int playerProfileId, int itemId, int quantity)
            => await _rewardDeliveryService.DeliverItemAsync(playerProfileId, itemId, quantity, "monster drop");

        // Normalizes world map names and maps aliases (such as ElfForest) to canonical map identifiers.
        private static string NormalizeMapName(string? mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))  // Mandatory string argument is blank — fail fast
                return "ElfForest";

            var normalized = mapName.Trim();
            return string.Equals(normalized, "ElfForest", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "ElfLand", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Map1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Chapter1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Chapter 1", StringComparison.OrdinalIgnoreCase)
                    ? "ElfForest"
                    : normalized;
        }
    }
}
