using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class SaleService : ISaleService
    {
        private readonly IPurchaseHistoryRepository _repository;
        private readonly IMapper _mapper;

        public SaleService(IPurchaseHistoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PurchaseHistoryResponseDto>> GetAllSales()
        {
            var purchases = await _repository.GetAllPurchaseHistories();

            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);
        }

        public async Task<List<PurchaseHistoryResponseDto>> GetSalesByPlayerId(int playerProfileId)
        {
            var purchases = await _repository.GetPurchasesByPlayerId(playerProfileId);

            return _mapper.Map<List<PurchaseHistoryResponseDto>>(purchases);
        }


    }
}
