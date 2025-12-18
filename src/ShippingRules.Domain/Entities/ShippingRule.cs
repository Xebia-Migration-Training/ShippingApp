namespace ShippingRules.Domain.Entities;

public class ShippingRule : BaseEntity
{
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Precedence factors (1 = highest precedence, 5 = lowest)
    public int PrecedenceLevel { get; set; }
    
    // Rule criteria - nullable for hierarchy precedence
    public Guid? CountryId { get; set; }
    public Guid? PortId { get; set; }
    public Guid? VesselId { get; set; }
    public Guid? PrincipalId { get; set; }
    
    // Effective date range
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    
    // Rule configuration
    public decimal BaseRate { get; set; }
    public decimal SurchargePercentage { get; set; }
    public string RuleType { get; set; } = string.Empty; // Freight, Customs, Port Charges, etc.
    public string RuleCategory { get; set; } = string.Empty;
    
    // Business validation
    public bool RequiresApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    // Navigation properties
    public Country? Country { get; set; }
    public Port? Port { get; set; }
    public Vessel? Vessel { get; set; }
    public Principal? Principal { get; set; }
}
