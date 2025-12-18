namespace ShippingRules.Domain.Entities;

public class Principal : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Vessel> Vessels { get; set; } = new List<Vessel>();
    public ICollection<ShippingRule> ShippingRules { get; set; } = new List<ShippingRule>();
}
