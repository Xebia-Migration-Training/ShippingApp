using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShippingRules.Application.DTOs;
using ShippingRules.MAUI.Services;

namespace ShippingRules.MAUI.ViewModels;

public partial class ValidateRuleViewModel : ObservableObject
{
    private readonly ShippingRulesApiService _apiService;

    [ObservableProperty]
    private Guid? _selectedCountryId;

    [ObservableProperty]
    private Guid? _selectedPortId;

    [ObservableProperty]
    private Guid? _selectedVesselId;

    [ObservableProperty]
    private Guid? _selectedPrincipalId;

    [ObservableProperty]
    private ShippingRuleDto? _applicableRule;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private string _validationMessage = string.Empty;

    public ValidateRuleViewModel(ShippingRulesApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task FindApplicableRuleAsync()
    {
        if (IsSearching) return;

        IsSearching = true;
        ValidationMessage = string.Empty;
        ApplicableRule = null;

        try
        {
            var rule = await _apiService.GetApplicableRuleAsync(
                SelectedCountryId,
                SelectedPortId,
                SelectedVesselId,
                SelectedPrincipalId);

            if (rule != null)
            {
                ApplicableRule = rule;
                ValidationMessage = $"✓ Found applicable rule: {rule.RuleName} (Precedence Level: {rule.PrecedenceLevel})";
            }
            else
            {
                ValidationMessage = "✗ No applicable rule found for the given criteria";
            }
        }
        catch (Exception ex)
        {
            ValidationMessage = $"✗ Error: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", $"Failed to find applicable rule: {ex.Message}", "OK");
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectedCountryId = null;
        SelectedPortId = null;
        SelectedVesselId = null;
        SelectedPrincipalId = null;
        ApplicableRule = null;
        ValidationMessage = string.Empty;
    }
}
