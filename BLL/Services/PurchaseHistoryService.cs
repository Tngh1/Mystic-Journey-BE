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
    public class PurchaseHistoryService : IPurchaseHistoryService
    {
        private readonly IPurchaseHistoryRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseHistoryService(IPurchaseHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PurchaseHistoryResponseDto>> GetAllPurchaseHistories()
        {
            var purchases = await _repository.GetAllPurchaseHistories();
            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);
        }

        public async Task<List<PurchaseHistoryResponseDto>> GetPurchasesByPlayerId(int playerProfileId)
        {
            var purchases = await _repository.GetPurchasesByPlayerId(playerProfileId);
            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);
        }



        public async Task<PagedResultDto<PurchaseHistoryResponseDto>> GetPurchaseHistoriesPaged(int page, int pageSize, string? search = null, string? sortBy = null, string? sortOrder = null)
        {
            var result = await _repository.GetPurchaseHistoriesPaged(page, pageSize, search, sortBy, sortOrder);
            var dtos = _mapper.Map<List<PurchaseHistoryResponseDto>>(result.Histories);
            return new PagedResultDto<PurchaseHistoryResponseDto>(result.TotalCount, dtos);
        }
    }
}
