using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShippingRules.Application.DTOs;
using ShippingRules.Application.Features.ShippingRules.Commands.CreateShippingRule;
using ShippingRules.Application.Features.ShippingRules.Queries.GetAllRules;
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

    public ShippingRulesController(
        IMediator mediator,
        RulePrecedenceService precedenceService,
        ILogger<ShippingRulesController> logger)
    {
        _mediator = mediator;
        _precedenceService = precedenceService;
        _logger = logger;
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
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for shipping rule");
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
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
        var rule = await _precedenceService.GetApplicableRuleAsync(
            request.CountryId,
            request.PortId,
            request.VesselId,
            request.PrincipalId,
            request.EffectiveDate ?? DateTime.UtcNow,
            request.RuleType ?? "FreightCharge");

        if (rule == null)
        {
            return NotFound(new { message = "No applicable rule found" });
        }

        var totalCost = _precedenceService.CalculateCost(rule, request.BaseAmount);

        return Ok(new
        {
            appliedRule = rule.RuleName,
            precedenceLevel = rule.PrecedenceLevel,
            baseRate = rule.BaseRate,
            surchargePercentage = rule.SurchargePercentage,
            baseAmount = request.BaseAmount,
            calculatedCost = totalCost
        });
    }
}

public record CostCalculationRequest(
    decimal BaseAmount,
    Guid? CountryId,
    Guid? PortId,
    Guid? VesselId,
    Guid? PrincipalId,
    DateTime? EffectiveDate,
    string? RuleType);
