using ShippingRules.MAUI.ViewModels;

namespace ShippingRules.MAUI.Views;

public partial class CreateRulePage : ContentPage
{
    public CreateRulePage(CreateRuleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
