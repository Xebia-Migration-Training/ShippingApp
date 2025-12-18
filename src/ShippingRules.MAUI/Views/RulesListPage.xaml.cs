using ShippingRules.MAUI.ViewModels;

namespace ShippingRules.MAUI.Views;

public partial class RulesListPage : ContentPage
{
    private readonly RulesListViewModel _viewModel;

    public RulesListPage(RulesListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRulesCommand.ExecuteAsync(null);
    }
}
