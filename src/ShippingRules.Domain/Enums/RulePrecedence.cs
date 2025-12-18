namespace ShippingRules.Domain.Enums;

/// <summary>
/// Defines the precedence hierarchy for shipping rules
/// Lower values have higher precedence
/// </summary>
public enum RulePrecedence
{
    VesselSpecific = 1,      // Highest precedence - specific vessel
    PortSpecific = 2,        // Port-level rules
    PrincipalSpecific = 3,   // Principal/carrier level rules
    CountrySpecific = 4,     // Country-level rules
    Global = 5               // Lowest precedence - global default rules
}
