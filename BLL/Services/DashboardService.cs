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
    public class DashboardService : IDashboardService
    {
        private readonly MysticJourneyDbContext _context;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMonsterRepository _monsterRepository;

        public DashboardService(
            MysticJourneyDbContext context,
            IPlayerProfileRepository playerProfileRepository,
            IAuthRepository authRepository,
            IItemRepository itemRepository,
            IMonsterRepository monsterRepository)
        {
            _context = context;
            _playerProfileRepository = playerProfileRepository;
            _authRepository = authRepository;
            _itemRepository = itemRepository;
            _monsterRepository = monsterRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStats()
        {
            var totalPlayers = await _playerProfileRepository.GetTotalPlayerProfilesCount();
            var totalAccounts = await _authRepository.GetTotalAccountsCount();
            var totalItems = await _context.Items.CountAsync();
            var totalMonsters = await _context.Monsters.CountAsync();
            var totalTransactions = await _context.PurchaseHistories.CountAsync();

            var totalRevenue = await _context.PurchaseHistories
                .SumAsync(p => p.TotalPrice);

            var monthlyStats = await GetMonthlyStatsAsync();

            return new DashboardStatsDto
            {
                TotalPlayers = totalPlayers,
                TotalAccounts = totalAccounts,
                TotalItems = totalItems,
                TotalMonsters = totalMonsters,
                TotalTransactions = totalTransactions,
                TotalRevenue = totalRevenue,
                MonthlyStats = monthlyStats
            };
        }

        private async Task<List<MonthlyStatDto>> GetMonthlyStatsAsync()
        {
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);

            var purchases = await _context.PurchaseHistories
                .Where(p => p.PurchasedAt >= twelveMonthsAgo)
                .ToListAsync();

            var groupedStats = purchases
                .GroupBy(p => new { p.PurchasedAt.Year, p.PurchasedAt.Month })
                .Select(g => new MonthlyStatDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count(),
                    Amount = g.Sum(p => p.TotalPrice)
                })
                .OrderBy(s => s.Month)
                .ToList();

            return groupedStats;
        }
    }
}
