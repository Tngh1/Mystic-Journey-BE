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
    // Executes core business logic for i purchase history service.
    public class PurchaseHistoryService : IPurchaseHistoryService
    {
        private readonly IPurchaseHistoryRepository _repository;
        private readonly IMapper _mapper;

        // Initializes a new instance of PurchaseHistoryService with dependencies: repository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PurchaseHistoryService(IPurchaseHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Load all purchase histories; it builds map.
        public async Task<List<PurchaseHistoryResponseDto>> GetAllPurchaseHistories()
        {
            var purchases = await _repository.GetAllPurchaseHistories();
            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get purchases by player id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed List<PurchaseHistoryResponseDto result asynchronously.
        public async Task<List<PurchaseHistoryResponseDto>> GetPurchasesByPlayerId(int playerProfileId)
        {
            var purchases = await _repository.GetPurchasesByPlayerId(playerProfileId);
            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);  // Transform domain entity into DTO for the API response layer
        }



        // Executes core business logic for get purchase histories paged.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed PagedResultDto<PurchaseHistoryResponseDto result asynchronously.
        public async Task<PagedResultDto<PurchaseHistoryResponseDto>> GetPurchaseHistoriesPaged(int page, int pageSize, string? search = null, string? sortBy = null, string? sortOrder = null)
        {
            var result = await _repository.GetPurchaseHistoriesPaged(page, pageSize, search, sortBy, sortOrder);
            var dtos = _mapper.Map<List<PurchaseHistoryResponseDto>>(result.Histories);  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<PurchaseHistoryResponseDto>(result.TotalCount, dtos);
        }
    }
}
