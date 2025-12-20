using ShippingRules.Domain.Entities;

namespace ShippingRules.Application.Interfaces;

public interface IExchangeRateRepository
{
    Task<ExchangeRate?> GetRateAsync(string fromCurrency, string toCurrency, DateTime at, CancellationToken ct = default);
}
