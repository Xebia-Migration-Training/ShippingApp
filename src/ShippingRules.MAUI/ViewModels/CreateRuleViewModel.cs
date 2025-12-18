using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShippingRules.MAUI.Services;

namespace ShippingRules.MAUI.ViewModels;

public partial class CreateRuleViewModel : ObservableObject
{
    private readonly ShippingRulesApiService _apiService;

    [ObservableProperty]
    private string _ruleName = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private int _precedenceLevel = 1;

    [ObservableProperty]
    private decimal _baseRate;

    [ObservableProperty]
    private decimal _surchargePercentage;

    [ObservableProperty]
    private DateTime _effectiveFrom = DateTime.Today;

    [ObservableProperty]
    private DateTime? _effectiveTo;

    [ObservableProperty]
    private string _ruleType = "FreightCharge";

    [ObservableProperty]
    private string _ruleCategory = string.Empty;

    [ObservableProperty]
    private bool _requiresApproval;

    [ObservableProperty]
    private bool _isSubmitting;

    public List<int> PrecedenceLevels => new() { 1, 2, 3, 4, 5 };
    public List<string> RuleTypes => new() 
    { 
        "FreightCharge", 
        "PortCharge", 
        "CustomsDuty", 
        "Surcharge", 
        "TerminalHandling", 
        "Documentation", 
        "Insurance" 
    };

    public CreateRuleViewModel(ShippingRulesApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task CreateRuleAsync()
    {
        if (IsSubmitting) return;

        if (string.IsNullOrWhiteSpace(RuleName))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Rule name is required", "OK");
            return;
        }

        IsSubmitting = true;
        try
        {
            var request = new CreateRuleRequest(
                RuleName,
                Description,
                PrecedenceLevel,
                null, // CountryId - can be added with dropdown
                null, // PortId
                null, // VesselId
                null, // PrincipalId
                EffectiveFrom,
                EffectiveTo,
                BaseRate,
                SurchargePercentage,
                RuleType,
                RuleCategory,
                RequiresApproval);

            var result = await _apiService.CreateRuleAsync(request);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Success", "Rule created successfully", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to create rule", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error creating rule: {ex.Message}", "OK");
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
