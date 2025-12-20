using ShippingRules.Application.Interfaces;

namespace ShippingRules.Application.Services;

public class ExchangeRateService
{
    private readonly IExchangeRateRepository _repo;

    public ExchangeRateService(IExchangeRateRepository repo)
    {
        _repo = repo;
    }

    public async Task<decimal?> TryGetRateAsync(string fromCurrency, string toCurrency, DateTime at, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fromCurrency) || string.IsNullOrWhiteSpace(toCurrency))
        {
            return null;
        }

        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return 1m;
        }

        var rate = await _repo.GetRateAsync(fromCurrency, toCurrency, at, ct);
        return rate?.Rate;
    }
}
