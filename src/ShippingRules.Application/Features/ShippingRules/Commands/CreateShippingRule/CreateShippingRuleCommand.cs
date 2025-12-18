using MediatR;
using ShippingRules.Application.DTOs;

namespace ShippingRules.Application.Features.ShippingRules.Commands.CreateShippingRule;

public record CreateShippingRuleCommand : IRequest<ShippingRuleDto>
{
    public string RuleName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int PrecedenceLevel { get; init; }
    
    public Guid? CountryId { get; init; }
    public Guid? PortId { get; init; }
    public Guid? VesselId { get; init; }
    public Guid? PrincipalId { get; init; }
    
    public DateTime EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    
    public decimal BaseRate { get; init; }
    public decimal SurchargePercentage { get; init; }
    public string RuleType { get; init; } = string.Empty;
    public string RuleCategory { get; init; } = string.Empty;
    
    public bool RequiresApproval { get; init; }
}
