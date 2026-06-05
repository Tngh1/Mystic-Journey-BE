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
    public class PurchaseHistoryService : IPurchaseHistoryService
    {
        private readonly MysticJourneyDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseHistoryService(MysticJourneyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PurchaseHistoryResponseDto>> GetAllPurchaseHistories()
        {
            var purchases = await _context.PurchaseHistories
                .Include(p => p.PlayerProfile)
                .Include(p => p.ShopItem)
                    .ThenInclude(s => s!.Item)
                .OrderByDescending(p => p.PurchasedAt)
                .ToListAsync();

            return purchases.Select(MapToResponseDto).ToList();
        }

        public async Task<List<PurchaseHistoryResponseDto>> GetPurchasesByPlayerId(int playerProfileId)
        {
            var purchases = await _context.PurchaseHistories
                .Include(p => p.PlayerProfile)
                .Include(p => p.ShopItem)
                    .ThenInclude(s => s!.Item)
                .Where(p => p.PlayerProfileId == playerProfileId)
                .OrderByDescending(p => p.PurchasedAt)
                .ToListAsync();

            return purchases.Select(MapToResponseDto).ToList();
        }

        private static PurchaseHistoryResponseDto MapToResponseDto(PurchaseHistory purchase)
        {
            return new PurchaseHistoryResponseDto
            {
                Id = purchase.PurchaseHistoryId,
                PlayerProfileId = purchase.PlayerProfileId,
                PlayerName = purchase.PlayerProfile?.DisplayName,
                ShopItemId = purchase.ShopItemId,
                ItemName = purchase.ShopItem?.Item?.Name,
                Quantity = purchase.Quantity,
                TotalPrice = purchase.TotalPrice,
                Currency = purchase.ShopItem?.Currency ?? "Unknown",
                PurchasedAt = purchase.PurchasedAt
            };
        }

        public IQueryable<PurchaseHistoryResponseDto> GetPurchaseHistoriesQueryable()
        {
            return _context.PurchaseHistories
                .Include(p => p.PlayerProfile)
                .Include(p => p.ShopItem)
                    .ThenInclude(s => s!.Item)
                .AsNoTracking()
                .Select(MapToResponseDto)
                .AsQueryable();
        }
    }
}
