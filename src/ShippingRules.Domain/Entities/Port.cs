namespace ShippingRules.Domain.Entities;

public class Port : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public string Type { get; set; } = string.Empty; // Seaport, Airport, etc.
    
    // Navigation properties
    public Country Country { get; set; } = null!;
    public ICollection<ShippingRule> ShippingRules { get; set; } = new List<ShippingRule>();
}
