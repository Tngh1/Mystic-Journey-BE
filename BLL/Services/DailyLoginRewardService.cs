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
    public class DailyLoginRewardService : IDailyLoginRewardService
    {
        private readonly IDailyLoginRewardRepository _repository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public DailyLoginRewardService(IDailyLoginRewardRepository repository, IItemRepository itemRepository, IMapper mapper)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        public async Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(CreateDailyLoginRewardRequestDto request)
        {
            var existingReward = await _repository.GetDailyLoginRewardByDayNumber(request.DayNumber);

            if (existingReward != null)
                throw new InvalidOperationException($"A reward for day {request.DayNumber} already exists.");

            var reward = new DailyLoginReward
            {
                DayNumber = request.DayNumber,
                RewardType = request.RewardType,
                RewardValue = request.RewardValue,
                RewardItemId = request.RewardItemId,
                RewardItemQuantity = request.RewardItemQuantity,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateDailyLoginReward(reward);

            if (reward.RewardItemId.HasValue)
            {
                var item = await _itemRepository.GetItemById(reward.RewardItemId.Value);
                reward.RewardItem = item;
            }

            return _mapper.Map<DailyLoginRewardResponseDto>(reward);
        }



        public async Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetDailyLoginRewardsPaged(page, pageSize);

            var dtos = _mapper.Map<List<DailyLoginRewardResponseDto>>(items);
            return new PagedResultDto<DailyLoginRewardResponseDto>(totalCount, dtos);
        }

        /// <summary>
        /// Trả về reward cho tất cả các ngày trong tháng hiện tại.
        /// Ngày nào chưa có trong DB sẽ được fill bằng placeholder IsActive=false.
        /// </summary>
        public async Task<List<DailyLoginRewardResponseDto>> GetCurrentMonthRewards()
        {
            var now = DateTime.UtcNow;
            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

            // Lấy tất cả reward từ DB (không phân trang)
            var (_, allItems) = await _repository.GetDailyLoginRewardsPaged(1, 400);
            var byDay = allItems
                .Where(r => r.DayNumber >= 1 && r.DayNumber <= daysInMonth)
                .ToDictionary(r => r.DayNumber);

            var result = new List<DailyLoginRewardResponseDto>(daysInMonth);
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (byDay.TryGetValue(day, out var reward))
                {
                    result.Add(_mapper.Map<DailyLoginRewardResponseDto>(reward));
                }
                else
                {
                    // Placeholder - ngày chưa được cấu hình reward
                    result.Add(new DailyLoginRewardResponseDto
                    {
                        DailyLoginRewardId = 0,
                        DayNumber = day,
                        RewardType = "None",
                        RewardValue = 0,
                        IsActive = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            return result;
        }
    }
}
