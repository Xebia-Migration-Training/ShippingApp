using ShippingRules.Domain.Entities;

namespace ShippingRules.Application.Exceptions;

public sealed class RuleConflictException : Exception
{
    public RuleConflictException(string message, ShippingRule candidate, IReadOnlyList<ShippingRule> conflicts)
        : base(message)
    {
        Candidate = candidate;
        Conflicts = conflicts;
    }

    public ShippingRule Candidate { get; }
    public IReadOnlyList<ShippingRule> Conflicts { get; }
}
