using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<DailyLoginRewardResponseDto>> GetAllDailyLoginRewards()
        {
            var rewards = await _repository.GetAllDailyLoginRewards();

            return rewards.Select(MapToResponseDto).ToList();
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

            return MapToResponseDto(reward);
        }

        private static DailyLoginRewardResponseDto MapToResponseDto(DailyLoginReward reward)
        {
            return new DailyLoginRewardResponseDto
            {
                DailyLoginRewardId = reward.DailyLoginRewardId,
                DayNumber = reward.DayNumber,
                RewardType = reward.RewardType,
                RewardValue = reward.RewardValue,
                RewardItemId = reward.RewardItemId,
                RewardItemName = reward.RewardItem?.Name,
                RewardItemQuantity = reward.RewardItemQuantity,
                IsActive = reward.IsActive
            };
        }

        public async Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetDailyLoginRewardsPaged(page, pageSize);

            var dtos = items.Select(MapToResponseDto).ToList();
            return new PagedResultDto<DailyLoginRewardResponseDto>(totalCount, dtos);
        }
    }
}
