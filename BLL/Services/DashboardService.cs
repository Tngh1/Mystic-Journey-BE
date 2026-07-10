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
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMonsterRepository _monsterRepository;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;

        public DashboardService(
            IPlayerProfileRepository playerProfileRepository,
            IAuthRepository authRepository,
            IItemRepository itemRepository,
            IMonsterRepository monsterRepository,
            IPurchaseHistoryRepository purchaseHistoryRepository)
        {
            _playerProfileRepository = playerProfileRepository;
            _authRepository = authRepository;
            _itemRepository = itemRepository;
            _monsterRepository = monsterRepository;
            _purchaseHistoryRepository = purchaseHistoryRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStats()
        {
            var totalPlayers = await _playerProfileRepository.GetTotalPlayerProfilesCount();
            var totalAccounts = await _authRepository.GetTotalAccountsCount();
            var totalItems = await _itemRepository.GetTotalItemsCount();
            var totalMonsters = await _monsterRepository.GetTotalMonstersCount();
            var totalTransactions = await _purchaseHistoryRepository.GetTotalTransactionsCount();

            var totalRevenue = await _purchaseHistoryRepository.GetTotalRevenue();

            var monthlyStats = await GetMonthlyStatsAsync();

            var onlineOfflineCounts = await GetOnlineOfflineCountsAsync();

            return new DashboardStatsDto
            {
                TotalPlayers = totalPlayers,
                TotalAccounts = totalAccounts,
                OnlinePlayers = onlineOfflineCounts.online,
                OfflinePlayers = onlineOfflineCounts.offline,
                TotalItems = totalItems,
                TotalMonsters = totalMonsters,
                TotalTransactions = totalTransactions,
                TotalRevenue = totalRevenue,
                MonthlyStats = monthlyStats
            };
        }

        private async Task<(int online, int offline)> GetOnlineOfflineCountsAsync()
        {
            var accounts = await _authRepository.GetAllActiveAccountsAsync();
            var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1);

            int online = 0;
            int offline = 0;

            foreach (var account in accounts)
            {
                if (account.LastSeen != null && account.LastSeen >= oneMinuteAgo)
                    online++;
                else
                    offline++;
            }

            return (online, offline);
        }

        private async Task<List<MonthlyStatDto>> GetMonthlyStatsAsync()
        {
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);

            var purchases = await _purchaseHistoryRepository.GetPurchasesSince(twelveMonthsAgo);

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
