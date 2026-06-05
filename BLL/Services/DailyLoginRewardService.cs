using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class DailyLoginRewardService : IDailyLoginRewardService
    {
        private readonly MysticJourneyDbContext _context;
        private readonly IMapper _mapper;

        public DailyLoginRewardService(MysticJourneyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<DailyLoginRewardResponseDto>> GetAllDailyLoginRewards()
        {
            var rewards = await _context.DailyLoginRewards
                .Include(r => r.RewardItem)
                .OrderBy(r => r.DayNumber)
                .ToListAsync();

            return rewards.Select(MapToResponseDto).ToList();
        }

        public async Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(CreateDailyLoginRewardRequestDto request)
        {
            var existingReward = await _context.DailyLoginRewards
                .FirstOrDefaultAsync(r => r.DayNumber == request.DayNumber);

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

            _context.DailyLoginRewards.Add(reward);
            await _context.SaveChangesAsync();

            if (reward.RewardItemId.HasValue)
            {
                var item = await _context.Items.FindAsync(reward.RewardItemId.Value);
                reward.RewardItem = item;
            }

            return MapToResponseDto(reward);
        }

        private static DailyLoginRewardResponseDto MapToResponseDto(DailyLoginReward reward)
        {
            return new DailyLoginRewardResponseDto
            {
                Id = reward.DailyLoginRewardId,
                DayNumber = reward.DayNumber,
                RewardType = reward.RewardType,
                RewardValue = reward.RewardValue,
                RewardItemId = reward.RewardItemId,
                RewardItemName = reward.RewardItem?.Name,
                RewardItemQuantity = reward.RewardItemQuantity,
                IsActive = reward.IsActive
            };
        }

        public IQueryable<DailyLoginRewardResponseDto> GetDailyLoginRewardsQueryable()
        {
            return _context.DailyLoginRewards
                .Include(r => r.RewardItem)
                .AsNoTracking()
                .Select(MapToResponseDto)
                .AsQueryable();
        }
    }
}
