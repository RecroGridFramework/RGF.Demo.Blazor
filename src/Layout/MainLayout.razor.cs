using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
#if DevExpressEnabled
using Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI;
#endif
using Recrovit.RecroGridFramework.Client.Blazor.Parameters;
using Recrovit.RecroGridFramework.Client.Blazor.RadzenUI;
using Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI;
#if TelerikEnabled
using Recrovit.RecroGridFramework.Client.Blazor.TelerikUI;
#endif
using Recrovit.RecroGridFramework.Client.Blazor.UI;

namespace RGF.Demo.Blazor.Layout;

public partial class MainLayout
{
    [Inject]
    IRecroDictService _recroDict { get; set; } = null!;

    [Inject]
    IJSRuntime _jsRuntime { get; set; } = null!;

    [Inject]
    IServiceProvider _serviceProvider { get; set; } = null!;

    [Inject]
    IConfiguration _configuration { get; set; } = null!;

    private bool _initialized { get; set; }

    private string _currentLanguage { get; set; } = string.Empty;

    private string _currentLibrary { get; set; } = "Bootstrap";

    private Type? _libraryWrapper { get; set; }

    private RenderFragment? _menu { get; set; }

    private RenderFragment? _themes { get; set; }

    private List<string> Libraries = new() { 
        "Bootstrap",
#if DevExpressEnabled
        "DevExpress",
#endif
        "Radzen",
        "Syncfusion",
#if TelerikEnabled
        "Telerik"
#endif
    };

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _currentLanguage = _recroDict.DefaultLanguage;
        await InitLibrary();
    }

    private void OnLanguageSelectionChanged(ChangeEventArgs args)
    {
        _initialized = false;
        StateHasChanged();

        _currentLanguage = args.Value?.ToString() ?? string.Empty;
        _ = Task.Run(async () =>
        {
            await _recroDict.SetDefaultLanguageAsync(_currentLanguage);
            _initialized = true;
            StateHasChanged();
        });
    }

    private void OnLibrarySelectionChanged(ChangeEventArgs args)
    {
        _initialized = false;
        StateHasChanged();

        var prev = _currentLibrary;
        _currentLanguage = args.Value?.ToString() ?? string.Empty;
        _ = Task.Run(async () =>
        {
            switch (prev)
            {
                case "Bootstrap":
                    await Recrovit.RecroGridFramework.Client.Blazor.UI.Configuration.UnloadResourcesAsync(_jsRuntime);
                    break;
#if DevExpressEnabled
                case "DevExpress":
                    await Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.Configuration.UnloadResourcesAsync(_jsRuntime);
                    break;
#endif
                case "Radzen":
                    await Recrovit.RecroGridFramework.Client.Blazor.RadzenUI.Configuration.UnloadResourcesAsync(_jsRuntime);
                    break;

                case "Syncfusion":
                    await Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI.Configuration.UnloadResourcesAsync(_jsRuntime);
                    break;
#if TelerikEnabled
                case "Telerik":
                    await Recrovit.RecroGridFramework.Client.Blazor.TelerikUI.Configuration.UnloadResourcesAsync(_jsRuntime);
                    break;
#endif
            }
            await _jsRuntime.InvokeVoidAsync("eval", "document.getElementsByTagName('html')[0].removeAttribute('class');");
            await _jsRuntime.InvokeVoidAsync("eval", "document.getElementsByTagName('body')[0].removeAttribute('class');");
            await _jsRuntime.InvokeVoidAsync("eval", "document.getElementsByTagName('body')[0].removeAttribute('style');");
            await InitLibrary();
            StateHasChanged();
        });
    }

    private async Task InitLibrary()
    {
        switch (_currentLibrary)
        {
            case "Bootstrap":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.UI.Components.RootComponent);
                await _serviceProvider.InitializeRgfUIAsync("light");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.UI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.UI.Components.SetTheme), "light");
                break;
#if DevExpressEnabled
            case "DevExpress":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.Components.DevExpressRootComponent);
                await _serviceProvider.InitializeRgfDevExpressUIAsync("blazing-berry.bs5");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.Components.SetTheme), "blazing-berry.bs5");
                break;
#endif
            case "Radzen":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.RadzenUI.Components.RadzenRootComponent);
                await _serviceProvider.InitializeRgfRadzenUIAsync("default");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.RadzenUI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.RadzenUI.Components.SetTheme), "default");
                break;

            case "Syncfusion":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI.Components.SyncfusionRootComponent);
                await _serviceProvider.InitializeRgfSyncfusionUIAsync("tailwind");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI.Components.SetTheme), "tailwind");
                break;
#if TelerikEnabled
            case "Telerik":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.TelerikUI.Components.TelerikRootComponent);
                bool trial = _configuration.GetValue<bool>("Telerik:Trial", true);
                await _serviceProvider.InitializeRgfTelerikUIAsync("kendo-theme-default/all", trial);
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.TelerikUI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.TelerikUI.Components.SetTheme), "kendo-theme-default/all");
                break;
#endif
        }
        await Task.Delay(1000);
        _initialized = true;
    }

    private void InitComponents(Type menu, Type? themes, string themeName)
    {
        _menu = (builder) =>
        {
            builder.OpenComponent(0, menu);
            builder.AddAttribute(1, "MenuParameters", new RgfMenuParameters() { MenuId = 10001, Navbar = true });
            builder.CloseComponent();
        };
        _themes = themes == null ? null : (builder) =>
        {
            builder.OpenComponent(0, themes);
            builder.AddAttribute(1, "ThemeName", themeName);
            builder.CloseComponent();
        };
    }
}
