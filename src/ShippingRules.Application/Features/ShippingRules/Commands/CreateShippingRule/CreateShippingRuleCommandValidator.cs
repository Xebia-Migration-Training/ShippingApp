using FluentValidation;

namespace ShippingRules.Application.Features.ShippingRules.Commands.CreateShippingRule;

public class CreateShippingRuleCommandValidator : AbstractValidator<CreateShippingRuleCommand>
{
    public CreateShippingRuleCommandValidator()
    {
        RuleFor(x => x.RuleName)
            .NotEmpty().WithMessage("Rule name is required")
            .MaximumLength(200).WithMessage("Rule name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.PrecedenceLevel)
            .InclusiveBetween(1, 5).WithMessage("Precedence level must be between 1 (highest) and 5 (lowest)");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty().WithMessage("Effective from date is required")
            .LessThan(x => x.EffectiveTo ?? DateTime.MaxValue)
            .WithMessage("Effective from must be before effective to date");

        RuleFor(x => x.BaseRate)
            .GreaterThanOrEqualTo(0).WithMessage("Base rate must be non-negative");

        RuleFor(x => x.SurchargePercentage)
            .InclusiveBetween(0, 100).WithMessage("Surcharge percentage must be between 0 and 100");

        RuleFor(x => x.RuleType)
            .NotEmpty().WithMessage("Rule type is required");

        // Business rule: At least one criterion must be specified
        RuleFor(x => x)
            .Must(x => x.CountryId.HasValue || x.PortId.HasValue || x.VesselId.HasValue || x.PrincipalId.HasValue)
            .WithMessage("At least one criterion (Country, Port, Vessel, or Principal) must be specified");

        // Precedence validation rules
        RuleFor(x => x)
            .Must(ValidatePrecedenceConsistency)
            .WithMessage("Precedence level must match the specificity of the rule");
    }

    private bool ValidatePrecedenceConsistency(CreateShippingRuleCommand command)
    {
        // Vessel-specific rules should have precedence 1
        if (command.VesselId.HasValue && command.PrecedenceLevel != 1)
            return false;

        // Port-specific rules should have precedence 2
        if (command.PortId.HasValue && !command.VesselId.HasValue && command.PrecedenceLevel != 2)
            return false;

        // Principal-specific rules should have precedence 3
        if (command.PrincipalId.HasValue && !command.VesselId.HasValue && !command.PortId.HasValue && command.PrecedenceLevel != 3)
            return false;

        // Country-specific rules should have precedence 4
        if (command.CountryId.HasValue && !command.PrincipalId.HasValue && !command.PortId.HasValue && !command.VesselId.HasValue && command.PrecedenceLevel != 4)
            return false;

        return true;
    }
}
