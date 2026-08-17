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
    // Executes core business logic for i daily login reward service.
    public class DailyLoginRewardService : IDailyLoginRewardService
    {
        private readonly IDailyLoginRewardRepository _repository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        // Initialize this instance from repository, item repository, and mapper and store repository, item repository, and mapper for later operations.
        public DailyLoginRewardService(
            IDailyLoginRewardRepository repository,
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }


        // Load daily login rewards paged using page, page size, month, and year; it builds map.
        public async Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null)
        {
            var (totalCount, items) = await _repository.GetDailyLoginRewardsPaged(page, pageSize, month, year);
            var dtos = _mapper.Map<List<DailyLoginRewardResponseDto>>(items);  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<DailyLoginRewardResponseDto>(totalCount, dtos);
        }

        // Load current month rewards using month and year; it loads overrides by month, loads all defaults, creates add, and builds map and guards invalid or unavailable states and processes each matching entry.
        public async Task<List<DailyLoginRewardResponseDto>> GetCurrentMonthRewards(
            int? month = null, int? year = null)
        {
            var now = DateTime.UtcNow;
            int targetMonth = month ?? now.Month;
            int targetYear  = year  ?? now.Year;
            int daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);

            var overrides = await _repository.GetOverridesByMonth(targetMonth, targetYear);
            var defaults  = await _repository.GetAllDefaults();

            var overrideByDay = overrides.ToDictionary(r => r.DayNumber);
            var defaultByDay  = defaults.ToDictionary(r => r.DayNumber);

            var result = new List<DailyLoginRewardResponseDto>(daysInMonth);

            for (int day = 1; day <= daysInMonth; day++)
            {
                if (overrideByDay.TryGetValue(day, out var overrideReward))
                {
                    result.Add(_mapper.Map<DailyLoginRewardResponseDto>(overrideReward));  // Transform domain entity into DTO for the API response layer
                }
                else if (defaultByDay.TryGetValue(day, out var defaultReward))
                {
                    result.Add(_mapper.Map<DailyLoginRewardResponseDto>(defaultReward));  // Transform domain entity into DTO for the API response layer
                }
                else
                {
                    result.Add(new DailyLoginRewardResponseDto
                    {
                        DailyLoginRewardId = 0,
                        DayNumber = day,
                        Month = null,
                        Year = null,
                        RewardType = "None",
                        RewardValue = 0,
                        IsActive = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            return result;
        }


        // Executes core business logic for get daily login reward by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed DailyLoginRewardResponseDto? result asynchronously.
        public async Task<DailyLoginRewardResponseDto?> GetDailyLoginRewardById(int id)
        {
            var reward = await _repository.GetDailyLoginRewardById(id);
            return reward == null ? null : _mapper.Map<DailyLoginRewardResponseDto>(reward);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get rewards by month.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed List<DailyLoginRewardResponseDto result asynchronously.
        public async Task<List<DailyLoginRewardResponseDto>> GetRewardsByMonth(int? month, int? year)
        {
            if (month == null || year == null)
            {
                var defaults = await _repository.GetAllDefaults();
                var defaultByDay = defaults.ToDictionary(r => r.DayNumber);
                const int MAX_DAYS = 31;

                return Enumerable.Range(1, MAX_DAYS).Select(day =>
                    defaultByDay.TryGetValue(day, out var r)
                        ? _mapper.Map<DailyLoginRewardResponseDto>(r)  // Transform domain entity into DTO for the API response layer
                        : MakePlaceholder(day, null, null)
                ).ToList();
            }

            return await GetCurrentMonthRewards(month, year);
        }

        // Create daily login reward using request; it loads default by day number, loads by day and month, loads item by id, and builds map and guards invalid or unavailable states.
        public async Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(
            CreateDailyLoginRewardRequestDto request)
        {
            if ((request.Month == null) != (request.Year == null))
                throw new InvalidOperationException("Month and Year must both be provided or both be null.");  // Unexpected runtime state — propagate to global error handler

            if (request.Month == null)
            {
                var existingDefault = await _repository.GetDefaultByDayNumber(request.DayNumber);
                if (existingDefault != null)  // Entity exists — proceed with conditional branch
                    throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                        $"A default reward for day {request.DayNumber} already exists. Please edit the existing one.");
            }
            else
            {
                var existingOverride = await _repository.GetByDayAndMonth(
                    request.DayNumber, request.Month.Value, request.Year!.Value);
                if (existingOverride != null)  // Entity exists — proceed with conditional branch
                    throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                        $"An override reward for day {request.DayNumber} in {request.Month}/{request.Year} already exists.");
            }

            var reward = new DailyLoginReward
            {
                DayNumber          = request.DayNumber,
                Month              = request.Month,
                Year               = request.Year,
                RewardType         = request.RewardType,
                RewardValue        = request.RewardValue,
                RewardItemId       = request.RewardItemId,
                RewardItemQuantity = request.RewardItemQuantity,
                IsActive           = request.IsActive,
                CreatedAt          = DateTime.UtcNow
            };

            await _repository.CreateDailyLoginReward(reward);

            if (reward.RewardItemId.HasValue)
            {
                var item = await _itemRepository.GetItemById(reward.RewardItemId.Value);
                reward.RewardItem = item;
            }

            return _mapper.Map<DailyLoginRewardResponseDto>(reward);  // Transform domain entity into DTO for the API response layer
        }

        // Update daily login reward using id and request; it loads daily login reward by id, loads item by id, and builds map and guards invalid or unavailable states.
        public async Task<DailyLoginRewardResponseDto> UpdateDailyLoginReward(
            int id, UpdateDailyLoginRewardRequestDto request)
        {
            var reward = await _repository.GetDailyLoginRewardById(id)
                ?? throw new KeyNotFoundException($"Daily login reward with ID {id} not found.");

            reward.RewardType         = request.RewardType;
            reward.RewardValue        = request.RewardValue;
            reward.RewardItemId       = request.RewardItemId;
            reward.RewardItemQuantity = request.RewardItemQuantity;
            reward.IsActive           = request.IsActive;

            await _repository.UpdateDailyLoginReward(reward);

            if (reward.RewardItemId.HasValue)
            {
                var item = await _itemRepository.GetItemById(reward.RewardItemId.Value);
                reward.RewardItem = item;
            }
            else
            {
                reward.RewardItem = null;
            }

            return _mapper.Map<DailyLoginRewardResponseDto>(reward);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for delete daily login reward.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        public async Task DeleteDailyLoginReward(int id)
        {
            var existing = await _repository.GetDailyLoginRewardById(id)
                ?? throw new KeyNotFoundException($"Daily login reward with ID {id} not found.");

            await _repository.DeleteDailyLoginReward(existing.DailyLoginRewardId);
        }


        // Executes core business logic for make placeholder.
        private static DailyLoginRewardResponseDto MakePlaceholder(int day, int? month, int? year) => new()
        {
            DailyLoginRewardId = 0,
            DayNumber          = day,
            Month              = month,
            Year               = year,
            RewardType         = "None",
            RewardValue        = 0,
            IsActive           = false,
            CreatedAt          = DateTime.UtcNow
        };
    }
}
