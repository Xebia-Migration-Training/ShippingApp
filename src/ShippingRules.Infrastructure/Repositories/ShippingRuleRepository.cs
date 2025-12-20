using Microsoft.EntityFrameworkCore;
using ShippingRules.Application.Interfaces;
using ShippingRules.Domain.Entities;
using ShippingRules.Infrastructure.Data;

namespace ShippingRules.Infrastructure.Repositories;

public class ShippingRuleRepository : IShippingRuleRepository
{
    private readonly ShippingRulesDbContext _context;

    public ShippingRuleRepository(ShippingRulesDbContext context)
    {
        _context = context;
    }

    public async Task<ShippingRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ShippingRules
            .Include(r => r.Country)
            .Include(r => r.Port)
            .Include(r => r.Vessel)
            .Include(r => r.Principal)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ShippingRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ShippingRules
            .Include(r => r.Country)
            .Include(r => r.Port)
            .Include(r => r.Vessel)
            .Include(r => r.Principal)
            .OrderBy(r => r.PrecedenceLevel)
            .ThenBy(r => r.RuleName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ShippingRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.ShippingRules
            .Include(r => r.Country)
            .Include(r => r.Port)
            .Include(r => r.Vessel)
            .Include(r => r.Principal)
            .Where(r => r.IsActive)
            .Where(r => r.EffectiveFrom <= now)
            .Where(r => !r.EffectiveTo.HasValue || r.EffectiveTo.Value >= now)
            .OrderBy(r => r.PrecedenceLevel)
            .ThenBy(r => r.RuleName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ShippingRule>> GetApplicableRulesAsync(
        Guid? countryId,
        Guid? portId,
        Guid? vesselId,
        Guid? principalId,
        DateTime effectiveDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ShippingRules
            .Include(r => r.Country)
            .Include(r => r.Port)
            .Include(r => r.Vessel)
            .Include(r => r.Principal)
            .Where(r => r.IsActive)
            .Where(r => r.EffectiveFrom <= effectiveDate)
            .Where(r => !r.EffectiveTo.HasValue || r.EffectiveTo.Value >= effectiveDate);

        // Build dynamic query based on precedence hierarchy
        var rules = new List<IQueryable<ShippingRule>>();

        // 1. Vessel-specific rules (highest precedence)
        if (vesselId.HasValue)
        {
            rules.Add(query.Where(r => r.VesselId == vesselId.Value));
        }

        // 2. Port-specific rules
        if (portId.HasValue)
        {
            rules.Add(query.Where(r => r.PortId == portId.Value && !r.VesselId.HasValue));
        }

        // 3. Principal-specific rules
        if (principalId.HasValue)
        {
            rules.Add(query.Where(r => r.PrincipalId == principalId.Value && !r.PortId.HasValue && !r.VesselId.HasValue));
        }

        // 4. Country-specific rules
        if (countryId.HasValue)
        {
            rules.Add(query.Where(r => r.CountryId == countryId.Value && !r.PrincipalId.HasValue && !r.PortId.HasValue && !r.VesselId.HasValue));
        }

        // 5. Global rules (lowest precedence)
        rules.Add(query.Where(r => !r.CountryId.HasValue && !r.PortId.HasValue && !r.VesselId.HasValue && !r.PrincipalId.HasValue));

        // Combine all queries
        var result = new List<ShippingRule>();
        foreach (var ruleQuery in rules)
        {
            result.AddRange(await ruleQuery.ToListAsync(cancellationToken));
        }

        return result.OrderBy(r => r.PrecedenceLevel);
    }

    public async Task<ShippingRule> AddAsync(ShippingRule rule, CancellationToken cancellationToken = default)
    {
        _context.ShippingRules.Add(rule);
        await _context.SaveChangesAsync(cancellationToken);
        return rule;
    }

    public async Task UpdateAsync(ShippingRule rule, CancellationToken cancellationToken = default)
    {
        rule.UpdatedAt = DateTime.UtcNow;
        _context.ShippingRules.Update(rule);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var rule = await GetByIdAsync(id, cancellationToken);
        if (rule != null)
        {
            _context.ShippingRules.Remove(rule);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> HasConflictingRuleAsync(ShippingRule rule, CancellationToken cancellationToken = default)
    {
        var newFrom = rule.EffectiveFrom;
        var newTo = rule.EffectiveTo ?? DateTime.MaxValue;

        return await _context.ShippingRules
            .Where(r => r.Id != rule.Id)
            .Where(r => r.IsActive)
            .Where(r => r.RuleType == rule.RuleType)
            .Where(r => r.CountryId == rule.CountryId)
            .Where(r => r.PortId == rule.PortId)
            .Where(r => r.VesselId == rule.VesselId)
            .Where(r => r.PrincipalId == rule.PrincipalId)
            .AnyAsync(r => r.EffectiveFrom <= newTo && newFrom <= (r.EffectiveTo ?? DateTime.MaxValue), cancellationToken);
    }

    public async Task<IReadOnlyList<ShippingRule>> GetConflictingRulesAsync(
        ShippingRule rule,
        CancellationToken cancellationToken = default)
    {
        var newFrom = rule.EffectiveFrom;
        var newTo = rule.EffectiveTo ?? DateTime.MaxValue;

        return await _context.ShippingRules
            .Include(r => r.Country)
            .Include(r => r.Port)
            .Include(r => r.Vessel)
            .Include(r => r.Principal)
            .Where(r => r.Id != rule.Id)
            .Where(r => r.IsActive)
            .Where(r => r.RuleType == rule.RuleType)
            .Where(r => r.CountryId == rule.CountryId)
            .Where(r => r.PortId == rule.PortId)
            .Where(r => r.VesselId == rule.VesselId)
            .Where(r => r.PrincipalId == rule.PrincipalId)
            .Where(r => r.EffectiveFrom <= newTo && newFrom <= (r.EffectiveTo ?? DateTime.MaxValue))
            .OrderBy(r => r.EffectiveFrom)
            .ToListAsync(cancellationToken);
    }
}
