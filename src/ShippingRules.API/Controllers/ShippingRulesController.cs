using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShippingRules.Application.DTOs;
using ShippingRules.Application.Exceptions;
using ShippingRules.Application.Features.ShippingRules.Commands.CreateShippingRule;
using ShippingRules.Application.Features.ShippingRules.Queries.GetAllRules;
using ShippingRules.Application.Interfaces;
using ShippingRules.Application.Services;

namespace ShippingRules.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ShippingRulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly RulePrecedenceService _precedenceService;
    private readonly ILogger<ShippingRulesController> _logger;
    private readonly IShippingRuleRepository _repository;
    private readonly ExchangeRateService _fx;

    public ShippingRulesController(
        IMediator mediator,
        RulePrecedenceService precedenceService,
        ILogger<ShippingRulesController> logger,
        IShippingRuleRepository repository,
        ExchangeRateService fx)
    {
        _mediator = mediator;
        _precedenceService = precedenceService;
        _logger = logger;
        _repository = repository;
        _fx = fx;
    }

    /// <summary>
    /// Get all shipping rules
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShippingRuleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ShippingRuleDto>>> GetAll([FromQuery] bool activeOnly = false)
    {
        _logger.LogInformation("Fetching all shipping rules. Active only: {ActiveOnly}", activeOnly);
        var result = await _mediator.Send(new GetAllRulesQuery { ActiveOnly = activeOnly });
        return Ok(result);
    }

    /// <summary>
    /// Create a new shipping rule
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ShippingRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShippingRuleDto>> Create([FromBody] CreateShippingRuleCommand command)
    {
        _logger.LogInformation("Creating new shipping rule: {RuleName}", command.RuleName);
        
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }
        catch (RuleConflictException ex)
        {
            static DateTime MaxDate(DateTime? dt) => dt ?? DateTime.MaxValue;
            static DateTime OverlapFrom(DateTime aFrom, DateTime bFrom) => aFrom > bFrom ? aFrom : bFrom;
            static DateTime OverlapTo(DateTime? aTo, DateTime? bTo)
            {
                var x = MaxDate(aTo);
                var y = MaxDate(bTo);
                return x < y ? x : y;
            }

            var details = ex.Conflicts.Select(c => new
            {
                id = c.Id,
                ruleName = c.RuleName,
                existingEffectiveFrom = c.EffectiveFrom,
                existingEffectiveTo = c.EffectiveTo,
                overlapFrom = OverlapFrom(c.EffectiveFrom, ex.Candidate.EffectiveFrom),
                overlapTo = OverlapTo(c.EffectiveTo, ex.Candidate.EffectiveTo)
            });

            return Conflict(new
            {
                message = ex.Message,
                suggestion = "Change EffectiveFrom/EffectiveTo, or expire/deactivate the existing rule, or use different criteria specificity.",
                conflicts = details
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for shipping rule");
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
    }

    public record ApproveRuleRequest(string? ApprovedBy);

    /// <summary>
    /// Approve a pending shipping rule (activates it)
    /// </summary>
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> ApproveRule(Guid id, [FromBody] ApproveRuleRequest request, CancellationToken ct)
    {
        var rule = await _repository.GetByIdAsync(id, ct);
        if (rule is null)
        {
            return NotFound(new { message = "Rule not found" });
        }

        rule.IsActive = true;
        rule.ApprovedBy = string.IsNullOrWhiteSpace(request.ApprovedBy) ? "System" : request.ApprovedBy.Trim();
        rule.ApprovedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(rule, ct);

        return Ok(new
        {
            message = "Rule approved",
            id = rule.Id,
            isActive = rule.IsActive,
            approvedBy = rule.ApprovedBy,
            approvedAt = rule.ApprovedAt
        });
    }

    /// <summary>
    /// Get the most applicable rule based on precedence logic
    /// </summary>
    [HttpGet("applicable")]
    [ProducesResponseType(typeof(ShippingRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShippingRuleDto>> GetApplicableRule(
        [FromQuery] Guid? countryId,
        [FromQuery] Guid? portId,
        [FromQuery] Guid? vesselId,
        [FromQuery] Guid? principalId,
        [FromQuery] DateTime? effectiveDate,
        [FromQuery] string ruleType = "FreightCharge")
    {
        _logger.LogInformation("Finding applicable rule for Country: {CountryId}, Port: {PortId}, Vessel: {VesselId}, Principal: {PrincipalId}", 
            countryId, portId, vesselId, principalId);

        var date = effectiveDate ?? DateTime.UtcNow;
        var rule = await _precedenceService.GetApplicableRuleAsync(
            countryId, portId, vesselId, principalId, date, ruleType);

        if (rule == null)
        {
            return NotFound(new { message = "No applicable rule found for the given criteria" });
        }

        var dto = new ShippingRuleDto
        {
            Id = rule.Id,
            RuleName = rule.RuleName,
            Description = rule.Description,
            PrecedenceLevel = rule.PrecedenceLevel,
            BaseRate = rule.BaseRate,
            SurchargePercentage = rule.SurchargePercentage,
            RuleType = rule.RuleType,
            RuleCategory = rule.RuleCategory,
            EffectiveFrom = rule.EffectiveFrom,
            EffectiveTo = rule.EffectiveTo,
            IsActive = rule.IsActive
        };

        return Ok(dto);
    }

    /// <summary>
    /// Calculate cost based on applicable rule
    /// </summary>
    [HttpPost("calculate-cost")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> CalculateCost(
        [FromBody] CostCalculationRequest request)
    {
        var effectiveAt = request.EffectiveDate ?? DateTime.UtcNow;
        var ruleType = request.RuleType ?? "FreightCharge";

        var rule = await _precedenceService.GetApplicableRuleAsync(
            request.CountryId,
            request.PortId,
            request.VesselId,
            request.PrincipalId,
            effectiveAt,
            ruleType);

        if (rule == null)
        {
            return NotFound(new { message = "No applicable rule found" });
        }

        var totalCost = _precedenceService.CalculateCost(rule, request.BaseAmount);

        // Minimal currency support: treat stored BaseRate as USD; optionally convert to TargetCurrency.
        const string baseCurrency = "USD";
        var target = string.IsNullOrWhiteSpace(request.TargetCurrency)
            ? baseCurrency
            : request.TargetCurrency.Trim().ToUpperInvariant();

        decimal? fxRate = null;
        decimal? convertedCost = null;

        if (!string.Equals(target, baseCurrency, StringComparison.OrdinalIgnoreCase))
        {
            fxRate = await _fx.TryGetRateAsync(baseCurrency, target, effectiveAt);
            if (fxRate is null)
            {
                return NotFound(new { message = "FX rate not found for conversion", from = baseCurrency, to = target });
            }

            convertedCost = totalCost * fxRate.Value;
        }

        return Ok(new
        {
            appliedRule = rule.RuleName,
            precedenceLevel = rule.PrecedenceLevel,
            baseRate = rule.BaseRate,
            surchargePercentage = rule.SurchargePercentage,
            baseAmount = request.BaseAmount,
            calculatedCost = totalCost,
            currency = baseCurrency,
            targetCurrency = target,
            fxRate,
            convertedCost
        });
    }

    public record BatchCostRequestItem(
        string Reference,
        decimal BaseAmount,
        Guid? CountryId,
        Guid? PortId,
        Guid? VesselId,
        Guid? PrincipalId,
        DateTime? EffectiveDate,
        string? RuleType);

    /// <summary>
    /// Batch simulation: calculate costs for many criteria rows
    /// </summary>
    [HttpPost("batch-calculate-cost")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> BatchCalculateCost([FromBody] List<BatchCostRequestItem> items)
    {
        var results = new List<object>(items.Count);

        foreach (var item in items)
        {
            var effectiveAt = item.EffectiveDate ?? DateTime.UtcNow;
            var ruleType = item.RuleType ?? "FreightCharge";

            var rule = await _precedenceService.GetApplicableRuleAsync(
                item.CountryId,
                item.PortId,
                item.VesselId,
                item.PrincipalId,
                effectiveAt,
                ruleType);

            if (rule is null)
            {
                results.Add(new { reference = item.Reference, found = false, message = "No applicable rule found" });
                continue;
            }

            var cost = _precedenceService.CalculateCost(rule, item.BaseAmount);
            results.Add(new
            {
                reference = item.Reference,
                found = true,
                appliedRule = rule.RuleName,
                precedenceLevel = rule.PrecedenceLevel,
                calculatedCost = cost
            });
        }

        return Ok(new { count = results.Count, results });
    }
/// Update the shipping rule by name
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ShippingRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShippingRuleDto>> Update(Guid id, [FromBody] CreateShippingRuleCommand command, CancellationToken ct)
    {
        var existingRule = await _repository.GetByIdAsync(id, ct);
        if (existingRule is null)
        {
            return NotFound(new { message = "Rule not found" });
        }

        existingRule.RuleName = command.RuleName;
        existingRule.Description = command.Description;
        existingRule.PrecedenceLevel = command.PrecedenceLevel;
        existingRule.BaseRate = command.BaseRate;
        existingRule.SurchargePercentage = command.SurchargePercentage;
        existingRule.RuleType = command.RuleType;
        existingRule.RuleCategory = command.RuleCategory;
        existingRule.EffectiveFrom = command.EffectiveFrom;
        existingRule.EffectiveTo = command.EffectiveTo;

        await _repository.UpdateAsync(existingRule, ct);

        var dto = new ShippingRuleDto
        {
            Id = existingRule.Id,
            RuleName = existingRule.RuleName,
            Description = existingRule.Description,
            PrecedenceLevel = existingRule.PrecedenceLevel,
            BaseRate = existingRule.BaseRate,
            SurchargePercentage = existingRule.SurchargePercentage,
            RuleType = existingRule.RuleType,
            RuleCategory = existingRule.RuleCategory,
            EffectiveFrom = existingRule.EffectiveFrom,
            EffectiveTo = existingRule.EffectiveTo,
            IsActive = existingRule.IsActive
        };

        _logger.LogInformation("Updated shipping rule with id: {RuleId}", id);

        return Ok(dto);
    }
// Delete the shipping rule by id
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existingRule = await _repository.GetByIdAsync(id, ct);
        if (existingRule is null)
        {
            return NotFound(new { message = "Rule not found" });
        }

        await _repository.DeleteAsync(id, ct);

        _logger.LogInformation("Deleted shipping rule with id: {RuleId}", id);

        return NoContent();
    }
      
    

}

public record CostCalculationRequest(
    decimal BaseAmount,
    Guid? CountryId,
    Guid? PortId,
    Guid? VesselId,
    Guid? PrincipalId,
    DateTime? EffectiveDate,
    string? RuleType,
    string? TargetCurrency = null);
