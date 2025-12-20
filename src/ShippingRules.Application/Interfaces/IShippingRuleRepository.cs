using ShippingRules.Domain.Entities;

namespace ShippingRules.Application.Interfaces;

public interface IShippingRuleRepository
{
    Task<ShippingRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShippingRule>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ShippingRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ShippingRule>> GetApplicableRulesAsync(
        Guid? countryId, 
        Guid? portId, 
        Guid? vesselId, 
        Guid? principalId,
        DateTime effectiveDate,
        CancellationToken cancellationToken = default);
    Task<ShippingRule> AddAsync(ShippingRule rule, CancellationToken cancellationToken = default);
    Task UpdateAsync(ShippingRule rule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasConflictingRuleAsync(ShippingRule rule, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShippingRule>> GetConflictingRulesAsync(
        ShippingRule rule,
        CancellationToken cancellationToken = default);
}
