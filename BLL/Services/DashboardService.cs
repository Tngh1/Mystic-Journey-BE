using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using BLL.Utils;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i dashboard service.
    public class DashboardService : IDashboardService
    {
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMonsterRepository _monsterRepository;
        private readonly IPurchaseHistoryRepository _purchaseHistoryRepository;

        // Initialize this instance from player profile repository, auth repository, item repository, and monster repository and store player profile repository, auth repository, item repository, monster repository, and purchase history repository for later operations.
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

        // Executes core business logic for get dashboard stats.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed DashboardStatsDto result asynchronously.
        public async Task<DashboardStatsDto> GetDashboardStats()
        {
            var totalPlayers = await _playerProfileRepository.GetTotalPlayerProfilesCount();
            var totalAccounts = await _authRepository.GetTotalAccountsCount();
            var totalItems = await _itemRepository.GetTotalItemsCount();
            var totalMonsters = await _monsterRepository.GetTotalMonstersCount();
            var totalTransactions = await _purchaseHistoryRepository.GetTotalTransactionsCount();

            var totalRevenue = await _purchaseHistoryRepository.GetTotalRevenue();

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
                TotalRevenue = totalRevenue
            };
        }

        // Executes core business logic for get online offline counts async.
        // Logic details: delegates data queries and updates to repository layer.
        private async Task<(int online, int offline)> GetOnlineOfflineCountsAsync()
        {
            var accounts = await _authRepository.GetAllActiveAccountsAsync();

            int online = 0;
            int offline = 0;

            foreach (var account in accounts)
            {
                if (OnlineTimeout.IsWithin(account.PlayerProfile?.LastSeen, OnlineTimeout.Dashboard))
                    online++;
                else
                    offline++;
            }

            return (online, offline);
        }
    }
}
