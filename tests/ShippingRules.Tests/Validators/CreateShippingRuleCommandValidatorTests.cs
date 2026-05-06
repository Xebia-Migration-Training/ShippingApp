using FluentAssertions;
using FluentValidation.TestHelper;
using ShippingRules.Application.Features.ShippingRules.Commands.CreateShippingRule;

namespace ShippingRules.Tests.Validators;

public class CreateShippingRuleCommandValidatorTests
{
    private readonly CreateShippingRuleCommandValidator _validator = new();

    private static CreateShippingRuleCommand ValidCommand() => new()
    {
        RuleName = "Test Rule",
        Description = "A valid test rule",
        PrecedenceLevel = 4,
        CountryId = Guid.NewGuid(),
        EffectiveFrom = DateTime.UtcNow,
        EffectiveTo = DateTime.UtcNow.AddMonths(6),
        BaseRate = 100m,
        SurchargePercentage = 5m,
        RuleType = "FreightCharge"
    };

    [Fact]
    public void ValidCommand_ShouldPassValidation()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyRuleName_ShouldFail()
    {
        var command = ValidCommand() with { RuleName = "" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RuleName);
    }

    [Fact]
    public void RuleNameExceedsMaxLength_ShouldFail()
    {
        var command = ValidCommand() with { RuleName = new string('A', 201) };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RuleName);
    }

    [Fact]
    public void NegativeBaseRate_ShouldFail()
    {
        var command = ValidCommand() with { BaseRate = -1m };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BaseRate);
    }

    [Fact]
    public void SurchargeOver100_ShouldFail()
    {
        var command = ValidCommand() with { SurchargePercentage = 101m };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SurchargePercentage);
    }

    [Fact]
    public void EffectiveFromAfterEffectiveTo_ShouldFail()
    {
        var command = ValidCommand() with
        {
            EffectiveFrom = DateTime.UtcNow.AddMonths(6),
            EffectiveTo = DateTime.UtcNow
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EffectiveFrom);
    }

    [Fact]
    public void NoCriteria_ShouldFail()
    {
        var command = ValidCommand() with
        {
            CountryId = null,
            PortId = null,
            VesselId = null,
            PrincipalId = null
        };
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void PrecedenceLevelOutOfRange_ShouldFail()
    {
        var command = ValidCommand() with { PrecedenceLevel = 6 };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PrecedenceLevel);
    }

    [Fact]
    public void VesselRuleWithWrongPrecedence_ShouldFail()
    {
        var command = ValidCommand() with
        {
            VesselId = Guid.NewGuid(),
            PrecedenceLevel = 3 // Should be 1 for vessel
        };
        var result = _validator.TestValidate(command);
        result.IsValid.Should().BeFalse();
    }
}
