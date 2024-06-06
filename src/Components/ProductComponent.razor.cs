using Microsoft.AspNetCore.Components;
using Recrovit.RecroGridFramework.Abstraction.Contracts.API;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
using Recrovit.RecroGridFramework.Abstraction.Models;
using Recrovit.RecroGridFramework.Client.Blazor.Components;
using Recrovit.RecroGridFramework.Client.Blazor.UI.Components;
using Recrovit.RecroGridFramework.Client.Events;
using Recrovit.RecroGridFramework.Client.Handlers;

namespace RGF.Demo.Blazor.Components;

public partial class ProductComponent : IDisposable
{
    [Inject]
    private ILogger<ProductComponent> _logger { get; set; } = null!;

    private EntityComponent _productRef { get; set; } = null!;

    private IRgManager Manager => EntityParameters.Manager ?? throw new MemberAccessException();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        EntityParameters.EventDispatcher.Subscribe(RgfEntityEventKind.Initialized, OnEntityInitialized);
        EntityParameters.ToolbarParameters.MenuEventDispatcher.Subscribe(OnMenuCommandAsync);

        EntityParameters.FormParameters.EventDispatcher.Subscribe(RgfFormEventKind.ValidationRequested, OnValidationRequested);
        EntityParameters.FormParameters.EventDispatcher.Subscribe(RgfFormEventKind.FormDataInitialized, (args) => _logger.LogInformation("EventKind: {0}", args.Args.EventKind));
        EntityParameters.FormParameters.OnSaveAsync = OnSaveAsync;
    }

    private void OnEntityInitialized(IRgfEventArgs<RgfEntityEventArgs> arg)
    {
        _logger.LogInformation("OnEntity{EventKind}", arg.Args.EventKind);
    }

    private Task OnMenuCommandAsync(IRgfEventArgs<RgfMenuEventArgs> arg)
    {
        var menuEventArgs = arg.Args;
        _logger.LogDebug("OnMenuCommand: {command}", menuEventArgs.Command);
        if (menuEventArgs.MenuType == RgfMenuType.Function)
        {
        }
        return Task.CompletedTask;
    }

    private Task<RgfResult<RgfFormResult>> OnSaveAsync(RgfFormComponent component, bool refresh)
    {
        _logger.LogInformation("OnSaveAsync");
        return component.OnSaveAsync(refresh);
    }

    private void OnValidationRequested(IRgfEventArgs<RgfFormEventArgs> arg)
    {
        _logger.LogInformation("EventKind: {0}, Alias: {1}", arg.Args.EventKind, arg.Args.Property?.Alias);
    }

    public void Dispose()
    {
        EntityParameters.ToolbarParameters.MenuEventDispatcher.Unsubscribe(OnMenuCommandAsync);
    }
}