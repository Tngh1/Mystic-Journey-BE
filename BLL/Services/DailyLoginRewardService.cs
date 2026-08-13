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
    // Implement IDailyLoginRewardService.
    //
    // Fallback priority khi lấy reward cho 1 ngày:
    //   1. Override (Month=m, Year=y, IsActive=true)
    //   2. Default  (Month=null, Year=null, IsActive=true)
    //   3. Placeholder (IsActive=false, IsDefault=true)
    public class DailyLoginRewardService : IDailyLoginRewardService
    {
        private readonly IDailyLoginRewardRepository _repository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public DailyLoginRewardService(
            IDailyLoginRewardRepository repository,
            IItemRepository itemRepository,
            IMapper mapper)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null)
        {
            var (totalCount, items) = await _repository.GetDailyLoginRewardsPaged(page, pageSize, month, year);
            var dtos = _mapper.Map<List<DailyLoginRewardResponseDto>>(items);
            return new PagedResultDto<DailyLoginRewardResponseDto>(totalCount, dtos);
        }

        /// <summary>
        /// Lấy reward cho từng ngày trong tháng với fallback:
        ///   override(day, month, year) → default(day) → placeholder
        /// Dùng cho cả game client (current-month) và admin (xem theo tháng).
        /// </summary>
        public async Task<List<DailyLoginRewardResponseDto>> GetCurrentMonthRewards(
            int? month = null, int? year = null)
        {
            var now = DateTime.UtcNow;
            int targetMonth = month ?? now.Month;
            int targetYear  = year  ?? now.Year;
            int daysInMonth = DateTime.DaysInMonth(targetYear, targetMonth);

            // Tải override và default (chạy tuần tự để tránh lỗi EF Core DbContext concurrent operation)
            var overrides = await _repository.GetOverridesByMonth(targetMonth, targetYear);
            var defaults  = await _repository.GetAllDefaults();

            var overrideByDay = overrides.ToDictionary(r => r.DayNumber);
            var defaultByDay  = defaults.ToDictionary(r => r.DayNumber);

            var result = new List<DailyLoginRewardResponseDto>(daysInMonth);

            for (int day = 1; day <= daysInMonth; day++)
            {
                if (overrideByDay.TryGetValue(day, out var overrideReward))
                {
                    // Override tháng/năm này
                    result.Add(_mapper.Map<DailyLoginRewardResponseDto>(overrideReward));
                }
                else if (defaultByDay.TryGetValue(day, out var defaultReward))
                {
                    // Fallback về default
                    result.Add(_mapper.Map<DailyLoginRewardResponseDto>(defaultReward));
                }
                else
                {
                    // Ngày chưa có reward nào — trả placeholder
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

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        public async Task<DailyLoginRewardResponseDto?> GetDailyLoginRewardById(int id)
        {
            var reward = await _repository.GetDailyLoginRewardById(id);
            return reward == null ? null : _mapper.Map<DailyLoginRewardResponseDto>(reward);
        }

        /// <summary>
        /// Lấy bộ rewards cho admin xem calendar theo tháng.
        /// month=null/year=null → trả default records.
        /// month+year có giá trị → trả overrides + fallback default cho ngày chưa override.
        /// </summary>
        public async Task<List<DailyLoginRewardResponseDto>> GetRewardsByMonth(int? month, int? year)
        {
            // Nếu xem tab "Default" (month=null) → trả đúng 31 ô default
            if (month == null || year == null)
            {
                var defaults = await _repository.GetAllDefaults();
                var defaultByDay = defaults.ToDictionary(r => r.DayNumber);
                const int MAX_DAYS = 31;

                return Enumerable.Range(1, MAX_DAYS).Select(day =>
                    defaultByDay.TryGetValue(day, out var r)
                        ? _mapper.Map<DailyLoginRewardResponseDto>(r)
                        : MakePlaceholder(day, null, null)
                ).ToList();
            }

            // Xem tháng cụ thể → override + fallback default
            return await GetCurrentMonthRewards(month, year);
        }

        public async Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(
            CreateDailyLoginRewardRequestDto request)
        {
            // Validate: Month và Year phải cùng null hoặc cùng có giá trị
            if ((request.Month == null) != (request.Year == null))
                throw new InvalidOperationException("Month and Year must both be provided or both be null.");

            // Check duplicate: cùng DayNumber + cùng Month + cùng Year
            if (request.Month == null)
            {
                // Kiểm tra duplicate default
                var existingDefault = await _repository.GetDefaultByDayNumber(request.DayNumber);
                if (existingDefault != null)
                    throw new InvalidOperationException(
                        $"A default reward for day {request.DayNumber} already exists. Please edit the existing one.");
            }
            else
            {
                // Kiểm tra duplicate override
                var existingOverride = await _repository.GetByDayAndMonth(
                    request.DayNumber, request.Month.Value, request.Year!.Value);
                if (existingOverride != null)
                    throw new InvalidOperationException(
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

            // Eager-load item name nếu có
            if (reward.RewardItemId.HasValue)
            {
                var item = await _itemRepository.GetItemById(reward.RewardItemId.Value);
                reward.RewardItem = item;
            }

            return _mapper.Map<DailyLoginRewardResponseDto>(reward);
        }

        public async Task<DailyLoginRewardResponseDto> UpdateDailyLoginReward(
            int id, UpdateDailyLoginRewardRequestDto request)
        {
            var reward = await _repository.GetDailyLoginRewardById(id)
                ?? throw new KeyNotFoundException($"Daily login reward with ID {id} not found.");

            // Cập nhật nội dung reward (không đổi DayNumber / Month / Year)
            reward.RewardType         = request.RewardType;
            reward.RewardValue        = request.RewardValue;
            reward.RewardItemId       = request.RewardItemId;
            reward.RewardItemQuantity = request.RewardItemQuantity;
            reward.IsActive           = request.IsActive;

            await _repository.UpdateDailyLoginReward(reward);

            // Reload item name
            if (reward.RewardItemId.HasValue)
            {
                var item = await _itemRepository.GetItemById(reward.RewardItemId.Value);
                reward.RewardItem = item;
            }
            else
            {
                reward.RewardItem = null;
            }

            return _mapper.Map<DailyLoginRewardResponseDto>(reward);
        }

        public async Task DeleteDailyLoginReward(int id)
        {
            var existing = await _repository.GetDailyLoginRewardById(id)
                ?? throw new KeyNotFoundException($"Daily login reward with ID {id} not found.");

            await _repository.DeleteDailyLoginReward(existing.DailyLoginRewardId);
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

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
