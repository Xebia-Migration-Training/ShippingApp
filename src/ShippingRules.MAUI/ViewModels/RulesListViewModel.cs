using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShippingRules.Application.DTOs;
using ShippingRules.MAUI.Services;
using System.Collections.ObjectModel;

namespace ShippingRules.MAUI.ViewModels;

public partial class RulesListViewModel : ObservableObject
{
    private readonly ShippingRulesApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<ShippingRuleDto> _rules = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _showActiveOnly = true;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public RulesListViewModel(ShippingRulesApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadRulesAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        try
        {
            var rules = await _apiService.GetAllRulesAsync(ShowActiveOnly);
            
            Rules.Clear();
            foreach (var rule in rules)
            {
                Rules.Add(rule);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load rules: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToCreateRuleAsync()
    {
        await Shell.Current.GoToAsync("CreateRulePage");
    }

    [RelayCommand]
    private async Task NavigateToValidateRuleAsync()
    {
        await Shell.Current.GoToAsync("ValidateRulePage");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadRulesAsync();
    }

    partial void OnShowActiveOnlyChanged(bool value)
    {
        _ = LoadRulesAsync();
    }
}
