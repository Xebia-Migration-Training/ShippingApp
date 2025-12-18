using ShippingRules.Application.Interfaces;
using ShippingRules.Domain.Entities;
using ShippingRules.Domain.ValueObjects;
using ShippingRules.Domain.Enums;

namespace ShippingRules.Application.Services;

public class RulePrecedenceService
{
    private readonly IShippingRuleRepository _repository;

    public RulePrecedenceService(IShippingRuleRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Validates a shipping rule against business rules and precedence logic
    /// </summary>
    public async Task<ValidationOutcome> ValidateRuleAsync(
        ShippingRule rule,
        CancellationToken cancellationToken = default)
    {
        var outcome = new ValidationOutcome { Status = ValidationStatus.Valid };

        // Check effective date validity
        if (rule.EffectiveTo.HasValue && rule.EffectiveFrom >= rule.EffectiveTo.Value)
        {
            outcome.Status = ValidationStatus.Invalid;
            outcome.Messages.Add("Effective From date must be before Effective To date");
        }

        // Check if rule is currently active based on dates
        var now = DateTime.UtcNow;
        if (rule.EffectiveFrom > now)
        {
            outcome.Warnings.Add($"Rule will become effective on {rule.EffectiveFrom:yyyy-MM-dd}");
        }

        if (rule.EffectiveTo.HasValue && rule.EffectiveTo.Value < now)
        {
            outcome.Warnings.Add("Rule has expired");
        }

        // Check for conflicting rules
        var hasConflict = await _repository.HasConflictingRuleAsync(rule, cancellationToken);
        if (hasConflict)
        {
            outcome.Status = ValidationStatus.Warning;
            outcome.Warnings.Add("A similar rule with overlapping criteria and dates already exists");
        }

        // Validate precedence consistency
        var precedenceValidation = ValidatePrecedenceLevel(rule);
        if (!precedenceValidation.IsValid)
        {
            outcome.Status = ValidationStatus.Invalid;
            outcome.Messages.AddRange(precedenceValidation.Messages);
        }

        // Check approval requirement
        if (rule.RequiresApproval && string.IsNullOrEmpty(rule.ApprovedBy))
        {
            outcome.Status = ValidationStatus.PendingApproval;
            outcome.Messages.Add("Rule requires approval before activation");
        }

        return outcome;
    }

    /// <summary>
    /// Gets the most applicable rule based on precedence hierarchy
    /// </summary>
    public async Task<ShippingRule?> GetApplicableRuleAsync(
        Guid? countryId,
        Guid? portId,
        Guid? vesselId,
        Guid? principalId,
        DateTime effectiveDate,
        string ruleType,
        CancellationToken cancellationToken = default)
    {
        var applicableRules = await _repository.GetApplicableRulesAsync(
            countryId, portId, vesselId, principalId, effectiveDate, cancellationToken);

        // Filter by rule type and active status
        var filteredRules = applicableRules
            .Where(r => r.RuleType == ruleType && r.IsActive)
            .Where(r => r.EffectiveFrom <= effectiveDate)
            .Where(r => !r.EffectiveTo.HasValue || r.EffectiveTo.Value >= effectiveDate)
            .ToList();

        // Return the rule with highest precedence (lowest precedence number)
        return filteredRules
            .OrderBy(r => r.PrecedenceLevel)
            .FirstOrDefault();
    }

    /// <summary>
    /// Validates that the precedence level matches the rule's specificity
    /// </summary>
    private ValidationOutcome ValidatePrecedenceLevel(ShippingRule rule)
    {
        var outcome = ValidationOutcome.Success();

        // Determine expected precedence based on criteria
        int expectedPrecedence;
        string specificityLevel;

        if (rule.VesselId.HasValue)
        {
            expectedPrecedence = (int)RulePrecedence.VesselSpecific;
            specificityLevel = "Vessel-specific";
        }
        else if (rule.PortId.HasValue)
        {
            expectedPrecedence = (int)RulePrecedence.PortSpecific;
            specificityLevel = "Port-specific";
        }
        else if (rule.PrincipalId.HasValue)
        {
            expectedPrecedence = (int)RulePrecedence.PrincipalSpecific;
            specificityLevel = "Principal-specific";
        }
        else if (rule.CountryId.HasValue)
        {
            expectedPrecedence = (int)RulePrecedence.CountrySpecific;
            specificityLevel = "Country-specific";
        }
        else
        {
            expectedPrecedence = (int)RulePrecedence.Global;
            specificityLevel = "Global";
        }

        if (rule.PrecedenceLevel != expectedPrecedence)
        {
            outcome = ValidationOutcome.Failure(
                $"Precedence level mismatch: {specificityLevel} rules should have precedence level {expectedPrecedence}, but got {rule.PrecedenceLevel}");
        }
        else
        {
            outcome.Metadata["SpecificityLevel"] = specificityLevel;
            outcome.Metadata["ExpectedPrecedence"] = expectedPrecedence;
        }

        return outcome;
    }

    /// <summary>
    /// Calculates the total cost based on applicable rules
    /// </summary>
    public decimal CalculateCost(ShippingRule rule, decimal baseAmount)
    {
        var totalRate = rule.BaseRate + (baseAmount * rule.SurchargePercentage / 100);
        return totalRate;
    }
}
