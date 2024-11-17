using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
using Recrovit.RecroGridFramework.Client.Blazor;
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
using RGF.Demo.Blazor.Components;

namespace RGF.Demo.Blazor.Layout;

public partial class MainLayout
{
    [Inject]
    private IRecroDictService _recroDict { get; init; } = null!;

    [Inject]
    private IRecroSecService _recroSec { get; init; } = null!;

    [Inject]
    private IJSRuntime _jsRuntime { get; init; } = null!;

    [Inject]
    private IServiceProvider _serviceProvider { get; init; } = null!;

    [Inject]
    private IConfiguration _configuration { get; init; } = null!;

    private bool _initialized;
    private string _currentLibrary = "Bootstrap";
    private Type? _libraryWrapper;
    private RenderFragment? _menu;
    private RenderFragment? _themes;
    private readonly List<string> Libraries = [
        "Bootstrap",
#if DevExpressEnabled
        "DevExpress",
#endif
        "Radzen",
        "Syncfusion",
#if TelerikEnabled
        "Telerik"
#endif
    ];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var lib = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "RGF.Demo.Library");
        if (!string.IsNullOrEmpty(lib))
        {
            _currentLibrary = lib;
        }
        await InitLibrary();
        _recroSec.LanguageChangedEvent.Subscribe((arg) => Recreate());
    }

    private void Recreate()
    {
        _initialized = false;
        StateHasChanged();
        _ = Task.Run(() =>
        {
            _initialized = true;
            StateHasChanged();
        });
    }

    private void OnLanguageSelectionChanged(ChangeEventArgs args)
    {
        var language = args.Value?.ToString() ?? string.Empty;
        if (!string.IsNullOrEmpty(language))
        {
            _ = _recroSec.SetUserLanguageAsync(language);
        }
    }

    private void OnLibrarySelectionChanged(ChangeEventArgs args)
    {
        _initialized = false;
        StateHasChanged();

        var prev = _currentLibrary;
        _currentLibrary = args.Value?.ToString() ?? string.Empty;
        _ = Task.Run(async () =>
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "RGF.Demo.Library", _currentLibrary);
            switch (prev)
            {
                case "Bootstrap":
                    await RGFClientBlazorUIConfiguration.UnloadResourcesAsync(_jsRuntime);
                    RgfBlazorConfiguration.ClearEntityComponentTypes();
                    break;
#if DevExpressEnabled
                case "DevExpress":
                    await Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.RGFClientBlazorDevExpressConfiguration.UnloadResourcesAsync(_jsRuntime);
                    break;
#endif
                case "Radzen":
                    await RGFClientBlazorRadzenConfiguration.UnloadResourcesAsync(_jsRuntime);
                    break;

                case "Syncfusion":
                    await RGFClientBlazorSyncfusionConfiguration.UnloadResourcesAsync(_jsRuntime);
                    break;
#if TelerikEnabled
                case "Telerik":
                    await Recrovit.RecroGridFramework.Client.Blazor.TelerikUI.RGFClientBlazorTelerikConfiguration.UnloadResourcesAsync(_jsRuntime);
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
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.UI.Components.RgfRootComponent);
                await _serviceProvider.InitializeRgfUIAsync("light");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.UI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.UI.Components.SetTheme), "light");
                await _jsRuntime.InvokeVoidAsync("eval", $"$('body').addClass('small')");
                RegisterEntityComponent();
                break;
#if DevExpressEnabled
            case "DevExpress":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.Components.RgfRootComponent);
                await _serviceProvider.InitializeRgfDevExpressUIAsync("blazing-berry.bs5");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.DevExpressUI.Components.SetTheme), "blazing-berry.bs5");
                break;
#endif
            case "Radzen":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.RadzenUI.Components.RgfRootComponent);
                await _serviceProvider.InitializeRgfRadzenUIAsync("default");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.RadzenUI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.RadzenUI.Components.SetTheme), "default");
                break;

            case "Syncfusion":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI.Components.RgfRootComponent);
                await _serviceProvider.InitializeRgfSyncfusionUIAsync("tailwind");
                InitComponents(typeof(Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI.Components.MenuComponent), typeof(Recrovit.RecroGridFramework.Client.Blazor.SyncfusionUI.Components.SetTheme), "tailwind");
                break;
#if TelerikEnabled
            case "Telerik":
                _libraryWrapper = typeof(Recrovit.RecroGridFramework.Client.Blazor.TelerikUI.Components.RgfRootComponent);
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
            if (_currentLibrary == "Bootstrap")
            {
                builder.AddAttribute(2, "Size", "small");
            }
            builder.CloseComponent();
        };
    }

    public static void RegisterEntityComponent()
    {
        RgfBlazorConfiguration.RegisterEntityComponent<ProductComponent>("RG_Product_1");
        //RgfBlazorConfiguration.RegisterEntityComponent<OrderComponent>("RG_Orders_1");
        RgfBlazorConfiguration.RegisterEntityComponent<OrderDetailsComponent>("RG_OrderDetails_1");
    }
}
