namespace ShippingRules.Domain.Entities;

public class ExchangeRate : BaseEntity
{
    public string FromCurrency { get; set; } = "USD";
    public string ToCurrency { get; set; } = "USD";
    public decimal Rate { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
