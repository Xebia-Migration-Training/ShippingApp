using ShippingRules.MAUI.ViewModels;

namespace ShippingRules.MAUI.Views;

public partial class ValidateRulePage : ContentPage
{
    public ValidateRulePage(ValidateRuleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
