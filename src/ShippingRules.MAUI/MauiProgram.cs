using ShippingRules.MAUI.Services;
using ShippingRules.MAUI.ViewModels;
using ShippingRules.MAUI.Views;
using CommunityToolkit.Maui;

namespace ShippingRules.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services
        builder.Services.AddSingleton<ShippingRulesApiService>();

        // Register ViewModels
        builder.Services.AddTransient<RulesListViewModel>();
        builder.Services.AddTransient<CreateRuleViewModel>();
        builder.Services.AddTransient<ValidateRuleViewModel>();

        // Register Views
        builder.Services.AddTransient<RulesListPage>();
        builder.Services.AddTransient<CreateRulePage>();
        builder.Services.AddTransient<ValidateRulePage>();

        return builder.Build();
    }
}
