using MediatR;
using ShippingRules.Application.DTOs;

namespace ShippingRules.Application.Features.ShippingRules.Queries.GetAllRules;

public record GetAllRulesQuery : IRequest<IEnumerable<ShippingRuleDto>>
{
    public bool ActiveOnly { get; init; } = false;
}
