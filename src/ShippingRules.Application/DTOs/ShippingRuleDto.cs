namespace ShippingRules.Application.DTOs;

public class ShippingRuleDto
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PrecedenceLevel { get; set; }
    
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    
    public Guid? PortId { get; set; }
    public string? PortName { get; set; }
    
    public Guid? VesselId { get; set; }
    public string? VesselName { get; set; }
    
    public Guid? PrincipalId { get; set; }
    public string? PrincipalName { get; set; }
    
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    
    public decimal BaseRate { get; set; }
    public decimal SurchargePercentage { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public string RuleCategory { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
