namespace ShippingRules.Domain.Entities;

public class Vessel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string IMONumber { get; set; } = string.Empty;
    public string VesselType { get; set; } = string.Empty;
    public Guid PrincipalId { get; set; }
    
    // Navigation properties
    public Principal Principal { get; set; } = null!;
    public ICollection<ShippingRule> ShippingRules { get; set; } = new List<ShippingRule>();
}
