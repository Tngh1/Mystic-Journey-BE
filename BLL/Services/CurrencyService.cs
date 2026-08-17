using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using DAL.Repositories.Results;

namespace BLL.Services
{
    // Executes core business logic for i currency service.
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _repository;
        private readonly IMapper _mapper;

        // Initializes a new instance of CurrencyService with dependencies: repository, mapper.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public CurrencyService(ICurrencyRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Executes core business logic for get balance.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws BadRequestException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed CurrencyBalanceResponseDto result asynchronously.
        public async Task<CurrencyBalanceResponseDto> GetBalance(int playerProfileId)
        {
            EnsureAuthenticated(playerProfileId);

            var profile = await _repository.GetPlayerProfile(playerProfileId);
            if (profile == null)  // Entity not found — short-circuit with appropriate error result
                throw new KeyNotFoundException("Player profile not found.");

            return MapBalance(profile);
        }

        // Process spend currency using player profile id and request; it builds balance and builds map and guards invalid or unavailable states.
        public async Task<CurrencySpendResponseDto> SpendCurrency(
            int playerProfileId,
            SpendCurrencyRequestDto request)
        {
            EnsureAuthenticated(playerProfileId);

            // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
            var currency = NormalizeCurrency(request.Currency);
            var reason = NormalizeReason(request.Reason);

            if (request.Amount <= 0)
                throw new BadRequestException("Amount must be greater than 0.");  // Business rule violation — surface as 400 Bad Request

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
                Transaction = _mapper.Map<PlayerCurrencyLogResponseDto>(result.CurrencyLog)  // Transform domain entity into DTO for the API response layer
            };
        }

        // Executes core business logic for throw if spend failed.
        // Logic details: throws BadRequestException, InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        private static void ThrowIfSpendFailed(CurrencySpendResult result, string currency)
        {
            switch (result.Status)
            {
                case CurrencySpendStatus.Success:
                    return;
                case CurrencySpendStatus.PlayerNotFound:
                    throw new KeyNotFoundException("Player profile not found.");
                case CurrencySpendStatus.InvalidAmount:
                    throw new BadRequestException("Amount must be greater than 0.");  // Business rule violation — surface as 400 Bad Request
                case CurrencySpendStatus.UnsupportedCurrency:
                    throw new BadRequestException("Currency must be Gold or Gems.");  // Business rule violation — surface as 400 Bad Request
                case CurrencySpendStatus.InsufficientCurrency:
                    throw new BadRequestException($"Not enough {currency}.");  // Business rule violation — surface as 400 Bad Request
                default:
                    throw new InvalidOperationException("Spend currency failed.");  // Unexpected runtime state — propagate to global error handler
            }
        }

        // Executes core business logic for map balance.
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

        // Executes core business logic for ensure authenticated.
        // Logic details: validates numeric boundary constraints; throws UnauthorizedAccessException on invalid state or rule violations.
        private static void EnsureAuthenticated(int playerProfileId)
        {
            if (playerProfileId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");  // Authentication token is invalid or expired
        }

        // Executes core business logic for normalize currency.
        // Logic details: throws BadRequestException on invalid state or rule violations.
        private static string NormalizeCurrency(string? currency)
            => string.Equals(currency, "Gold", StringComparison.OrdinalIgnoreCase) ? "Gold"
            : string.Equals(currency, "Gems", StringComparison.OrdinalIgnoreCase) ? "Gems"
            : throw new BadRequestException("Currency must be Gold or Gems.");  // Business rule violation — surface as 400 Bad Request

        // Executes core business logic for normalize reason.
        // Logic details: validates required non-empty string arguments.
        private static string NormalizeReason(string? reason)
            => string.IsNullOrWhiteSpace(reason) ? "Spend" : reason.Trim();
    }
}
