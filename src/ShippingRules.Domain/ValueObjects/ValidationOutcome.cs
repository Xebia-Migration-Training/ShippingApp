using ShippingRules.Domain.Enums;

namespace ShippingRules.Domain.ValueObjects;

public class ValidationOutcome
{
    public ValidationStatus Status { get; set; }
    public List<string> Messages { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    public bool IsValid => Status == ValidationStatus.Valid;
    
    public static ValidationOutcome Success(string message = "Validation successful")
    {
        return new ValidationOutcome
        {
            Status = ValidationStatus.Valid,
            Messages = new List<string> { message }
        };
    }
    
    public static ValidationOutcome Failure(params string[] errors)
    {
        return new ValidationOutcome
        {
            Status = ValidationStatus.Invalid,
            Messages = errors.ToList()
        };
    }
    
    public static ValidationOutcome WithWarning(string message, params string[] warnings)
    {
        return new ValidationOutcome
        {
            Status = ValidationStatus.Warning,
            Messages = new List<string> { message },
            Warnings = warnings.ToList()
        };
    }
}
