﻿using Microsoft.AspNetCore.Components;
using Recrovit.RecroGridFramework.Abstraction.Contracts.API;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
using Recrovit.RecroGridFramework.Abstraction.Models;
using Recrovit.RecroGridFramework.Client.Blazor.Components;
using Recrovit.RecroGridFramework.Client.Blazor.Parameters;
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

    public RgfFormParameters FormParameter { get; set; } = new();

    public RgfGridParameters GridParameter { get; set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        EntityParameters.EventDispatcher.Subscribe(RgfEntityEventKind.Initialized, OnEntityInitialized);
        EntityParameters.ToolbarParameters.MenuEventDispatcher.Subscribe(OnMenuCommandAsync);

        //FormParameter.FormItemLayoutTemplate = (param) => @<div>@param.Property.Alias</div>;
        //GridParameter.ColumnTemplate = (param) => @<div>@param.PropDesc.Alias</div>;

        FormParameter.EventDispatcher.Subscribe(RgfFormEventKind.ValidationRequested, OnValidationRequested);
        FormParameter.EventDispatcher.Subscribe(RgfFormEventKind.FormDataInitialized, (args) => _logger.LogInformation("EventKind: {0}", args.Args.EventKind));
        FormParameter.OnSaveAsync = OnSaveAsync;
    }

    private void OnEntityInitialized(IRgfEventArgs<RgfEntityEventArgs> arg)
    {
        _logger.LogInformation("OnEntity{EventKind}", arg.Args.EventKind);
    }

    private async Task OnMenuCommandAsync(IRgfEventArgs<RgfMenuEventArgs> arg)
    {
        var menuItem = arg.Args;
        _logger.LogDebug("OnMenuCommand: {command}", menuItem.Command);
        if (menuItem.MenuType == RgfMenuType.Function)
        {
            var result = await Manager.ListHandler.CallCustomFunctionAsync(menuItem.Command, true);
            if (result != null)
            {
                arg.Handled = arg.PreventDefault = true;
                Manager.BroadcastMessages(result.Messages, this);
            }
            return;
        }
    }

    private async Task<RgfResult<RgfFormResult>> OnSaveAsync(RgfFormComponent component, bool refresh)
    {
        _logger.LogInformation("OnSaveAsync");
        return await component.OnSaveAsync(refresh);
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