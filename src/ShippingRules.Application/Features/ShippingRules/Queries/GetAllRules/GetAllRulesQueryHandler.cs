using MediatR;
using ShippingRules.Application.DTOs;
using ShippingRules.Application.Interfaces;

namespace ShippingRules.Application.Features.ShippingRules.Queries.GetAllRules;

public class GetAllRulesQueryHandler : IRequestHandler<GetAllRulesQuery, IEnumerable<ShippingRuleDto>>
{
    private readonly IShippingRuleRepository _repository;

    public GetAllRulesQueryHandler(IShippingRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ShippingRuleDto>> Handle(GetAllRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = request.ActiveOnly 
            ? await _repository.GetActiveRulesAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return rules.Select(rule => new ShippingRuleDto
        {
            Id = rule.Id,
            RuleName = rule.RuleName,
            Description = rule.Description,
            PrecedenceLevel = rule.PrecedenceLevel,
            CountryId = rule.CountryId,
            CountryName = rule.Country?.Name,
            PortId = rule.PortId,
            PortName = rule.Port?.Name,
            VesselId = rule.VesselId,
            VesselName = rule.Vessel?.Name,
            PrincipalId = rule.PrincipalId,
            PrincipalName = rule.Principal?.Name,
            EffectiveFrom = rule.EffectiveFrom,
            EffectiveTo = rule.EffectiveTo,
            BaseRate = rule.BaseRate,
            SurchargePercentage = rule.SurchargePercentage,
            RuleType = rule.RuleType,
            RuleCategory = rule.RuleCategory,
            IsActive = rule.IsActive,
            RequiresApproval = rule.RequiresApproval,
            ApprovedBy = rule.ApprovedBy,
            ApprovedAt = rule.ApprovedAt
        }).OrderBy(r => r.PrecedenceLevel).ToList();
    }
}
