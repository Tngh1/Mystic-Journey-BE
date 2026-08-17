using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i sale service.
    public class SaleService : ISaleService
    {
        private readonly IPurchaseHistoryRepository _repository;
        private readonly IMapper _mapper;

        // Initializes a new instance of SaleService with dependencies: repository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public SaleService(IPurchaseHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Executes core business logic for get all sales.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed List<PurchaseHistoryResponseDto result asynchronously.
        public async Task<List<PurchaseHistoryResponseDto>> GetAllSales()
        {
            var purchases = await _repository.GetAllPurchaseHistories();

            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for get sales by player id.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed List<PurchaseHistoryResponseDto result asynchronously.
        public async Task<List<PurchaseHistoryResponseDto>> GetSalesByPlayerId(int playerProfileId)
        {
            var purchases = await _repository.GetPurchasesByPlayerId(playerProfileId);

            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);  // Transform domain entity into DTO for the API response layer
        }


    }
}
