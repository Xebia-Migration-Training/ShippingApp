using MediatR;
using ShippingRules.Application.DTOs;
using ShippingRules.Application.Interfaces;
using ShippingRules.Domain.Entities;

namespace ShippingRules.Application.Features.ShippingRules.Commands.CreateShippingRule;

public class CreateShippingRuleCommandHandler : IRequestHandler<CreateShippingRuleCommand, ShippingRuleDto>
{
    private readonly IShippingRuleRepository _repository;

    public CreateShippingRuleCommandHandler(IShippingRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShippingRuleDto> Handle(CreateShippingRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = new ShippingRule
        {
            Id = Guid.NewGuid(),
            RuleName = request.RuleName,
            Description = request.Description,
            PrecedenceLevel = request.PrecedenceLevel,
            CountryId = request.CountryId,
            PortId = request.PortId,
            VesselId = request.VesselId,
            PrincipalId = request.PrincipalId,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            BaseRate = request.BaseRate,
            SurchargePercentage = request.SurchargePercentage,
            RuleType = request.RuleType,
            RuleCategory = request.RuleCategory,
            RequiresApproval = request.RequiresApproval,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System" // TODO: Get from authentication context
        };

        var createdRule = await _repository.AddAsync(rule, cancellationToken);

        return new ShippingRuleDto
        {
            Id = createdRule.Id,
            RuleName = createdRule.RuleName,
            Description = createdRule.Description,
            PrecedenceLevel = createdRule.PrecedenceLevel,
            CountryId = createdRule.CountryId,
            PortId = createdRule.PortId,
            VesselId = createdRule.VesselId,
            PrincipalId = createdRule.PrincipalId,
            EffectiveFrom = createdRule.EffectiveFrom,
            EffectiveTo = createdRule.EffectiveTo,
            BaseRate = createdRule.BaseRate,
            SurchargePercentage = createdRule.SurchargePercentage,
            RuleType = createdRule.RuleType,
            RuleCategory = createdRule.RuleCategory,
            IsActive = createdRule.IsActive,
            RequiresApproval = createdRule.RequiresApproval
        };
    }
}
