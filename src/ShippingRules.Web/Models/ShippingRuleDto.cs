namespace ShippingRules.Web.Models;

public class ShippingRuleDto
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PrecedenceLevel { get; set; }
    public decimal BaseRate { get; set; }
    public decimal SurchargePercentage { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
}