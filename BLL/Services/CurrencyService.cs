using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Results;

namespace BLL.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _repository;
        private readonly IMapper _mapper;

        public CurrencyService(ICurrencyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CurrencyBalanceResponseDto> GetBalance(int playerProfileId)
        {
            EnsureAuthenticated(playerProfileId);

            var profile = await _repository.GetPlayerProfile(playerProfileId);
            if (profile == null)
                throw new KeyNotFoundException("Player profile not found.");

            return MapBalance(profile);
        }

        public async Task<CurrencySpendResponseDto> SpendCurrency(
            int playerProfileId,
            SpendCurrencyRequestDto request)
        {
            EnsureAuthenticated(playerProfileId);

            var currency = NormalizeCurrency(request.Currency);
            var reason = NormalizeReason(request.Reason);

            if (request.Amount <= 0)
                throw new BadRequestException("Amount must be greater than 0.");

            var result = await _repository.TrySpendCurrency(
                playerProfileId,
                currency,
                request.Amount,
                reason,
                DateTime.UtcNow);

            ThrowIfSpendFailed(result, currency);

            return new CurrencySpendResponseDto
            {
                Success = true,
                Message = $"{currency} spent.",
                Currency = currency,
                AmountSpent = request.Amount,
                BalanceBefore = result.BalanceBefore,
                BalanceAfter = result.BalanceAfter,
                Balance = MapBalance(result.PlayerProfile!),
                Transaction = _mapper.Map<PlayerCurrencyLogResponseDto>(result.CurrencyLog)
            };
        }

        private static void ThrowIfSpendFailed(CurrencySpendResult result, string currency)
        {
            switch (result.Status)
            {
                case CurrencySpendStatus.Success:
                    return;
                case CurrencySpendStatus.PlayerNotFound:
                    throw new KeyNotFoundException("Player profile not found.");
                case CurrencySpendStatus.InvalidAmount:
                    throw new BadRequestException("Amount must be greater than 0.");
                case CurrencySpendStatus.UnsupportedCurrency:
                    throw new BadRequestException("Currency must be Gold or Gems.");
                case CurrencySpendStatus.InsufficientCurrency:
                    throw new BadRequestException($"Not enough {currency}.");
                default:
                    throw new InvalidOperationException("Spend currency failed.");
            }
        }

        private static CurrencyBalanceResponseDto MapBalance(PlayerProfile profile)
        {
            return new CurrencyBalanceResponseDto
            {
                PlayerProfileId = profile.PlayerProfileId,
                Gold = profile.Gold,
                Gems = profile.Gems,
                ServerTimeUtc = DateTime.UtcNow
            };
        }

        private static void EnsureAuthenticated(int playerProfileId)
        {
            if (playerProfileId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");
        }

        private static string NormalizeCurrency(string? currency)
            => string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase) ? "Gold"
            : string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase) ? "Gems"
            : throw new BadRequestException("Currency must be Gold or Gems.");

        private static string NormalizeReason(string? reason)
            => string.IsNullOrWhiteSpace(reason) ? "Spend" : reason.Trim();
    }
}
