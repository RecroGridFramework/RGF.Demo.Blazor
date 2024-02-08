using Microsoft.AspNetCore.Components;
using Recrovit.RecroGridFramework.Abstraction.Contracts.API;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
using Recrovit.RecroGridFramework.Abstraction.Models;
using Recrovit.RecroGridFramework.Client.Blazor.Components;
using Recrovit.RecroGridFramework.Client.Blazor.Events;
using Recrovit.RecroGridFramework.Client.Blazor.Parameters;
using Recrovit.RecroGridFramework.Client.Blazor.UI.Components;
using Recrovit.RecroGridFramework.Client.Events;
using Recrovit.RecroGridFramework.Client.Handlers;

namespace RGF.Demo.Blazor.Components;

public partial class ProductComponent
{
    [Inject]
    private ILogger<ProductComponent> _logger { get; set; } = null!;

    private EntityComponent _productRef { get; set; } = null!;

    private IRgManager Manager => EntityParameters.Manager ?? throw new MemberAccessException();

    public RgfFormParameters FormParameter { get; set; } = new();

    public RgfGridParameters GridParameter { get; set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        EntityParameters.EventDispatcher.Subscribe(RgfEntityEventKind.Initialized, OnEntityInitialized);
        //FormParameter.FormItemLayout = (param) => @<div>@param.Property.Alias</div>;
        //GridParameter.GridColumnTemplate = (param) => @<div>@param.PropDesc.Alias</div>;

        //FormParameter.EventDispatcher.Subscribe(FormViewEventKind.ValidationRequested, OnValidationRequested);
        //FormParameter.EventDispatcher.Subscribe(FormViewEventKind.FormDataInitialized, (sender, args) => _logger.LogInformation("EventKind: {0}", args.Args.EventKind));
        //FormParameter.OnSaveAsync = OnSaveAsync;
    }

    private void OnEntityInitialized(IRgfEventArgs<RgfEntityEventArgs> arg)
    {
        if (arg.Args.Manager!= null)
        {
            arg.Args.Manager.NotificationManager.Subscribe<RgfMenuEventArgs>(this, OnMenuCommandAsync);
        }
    }

    private async Task OnMenuCommandAsync(IRgfEventArgs<RgfMenuEventArgs> args)
    {
        var menuItem = args.Args;
        _logger.LogDebug("OnMenuCommand: {command}", menuItem.Command);
        if (menuItem.MenuType == RgfMenuType.Function)
        {
            var result = await Manager.ListHandler.CallCustomFunctionAsync(menuItem.Command, true);
            if (result != null)
            {
                Manager.BroadcastMessages(result.Messages, this);
            }
            return;
        }

        Manager.NotificationManager.RaiseEvent(new RgfUserMessage(Manager.RecroDict, UserMessageType.Error, "This type of menu option is currently not implemented."), this);
    }

    private async Task<RgfResult<RgfFormResult>> OnSaveAsync(RgfFormComponent component, bool refresh)
    {
        _logger.LogInformation("OnSaveAsync");
        return await component.OnSaveAsync(refresh);
    }

    private void OnValidationRequested(object? sender, IRgfEventArgs<RgfFormEventArgs> args)
    {
        _logger.LogInformation("EventKind: {0}, Alias: {1}", args.Args.EventKind, args.Args.Property?.Alias);
    }
}
