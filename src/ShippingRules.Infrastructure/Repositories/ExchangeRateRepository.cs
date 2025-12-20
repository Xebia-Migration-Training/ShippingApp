using Microsoft.EntityFrameworkCore;
using ShippingRules.Application.Interfaces;
using ShippingRules.Domain.Entities;
using ShippingRules.Infrastructure.Data;

namespace ShippingRules.Infrastructure.Repositories;

public class ExchangeRateRepository : IExchangeRateRepository
{
    private readonly ShippingRulesDbContext _db;

    public ExchangeRateRepository(ShippingRulesDbContext db)
    {
        _db = db;
    }

    public async Task<ExchangeRate?> GetRateAsync(string fromCurrency, string toCurrency, DateTime at, CancellationToken ct = default)
    {
        fromCurrency = fromCurrency.Trim().ToUpperInvariant();
        toCurrency = toCurrency.Trim().ToUpperInvariant();

        return await _db.ExchangeRates
            .Where(x => x.IsActive)
            .Where(x => x.FromCurrency == fromCurrency && x.ToCurrency == toCurrency)
            .Where(x => x.EffectiveFrom <= at)
            .Where(x => !x.EffectiveTo.HasValue || x.EffectiveTo.Value >= at)
            .OrderByDescending(x => x.EffectiveFrom)
            .FirstOrDefaultAsync(ct);
    }
}
