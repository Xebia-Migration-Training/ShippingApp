namespace ShippingRules.Domain.Entities;

public class Country : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Port> Ports { get; set; } = new List<Port>();
    public ICollection<ShippingRule> ShippingRules { get; set; } = new List<ShippingRule>();
}
