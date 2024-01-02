using Microsoft.AspNetCore.Components;
using Recrovit.RecroGridFramework.Abstraction.Contracts.API;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
using Recrovit.RecroGridFramework.Client.Blazor.Components;
using Recrovit.RecroGridFramework.Client.Blazor.Events;
using Recrovit.RecroGridFramework.Client.Blazor.Parameters;
using Recrovit.RecroGridFramework.Client.Blazor.UI.Components;

namespace RGF.Demo.Blazor.Components;

public partial class ProductComponent
{
    [Inject]
    private ILogger<ProductComponent> _logger { get; set; } = null!;

    private EntityComponent _productRef { get; set; } = null!;

    public RgfFormParameters FormParameter { get; set; } = new();

    public RgfGridParameters GridParameter { get; set; } = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        //FormParameter.FormItemLayout = (param) => @<div>@param.Property.Alias</div>;
        //GridParameter.GridColumnTemplate = (param) => @<div>@param.PropDesc.Alias</div>;

        //FormParameter.EventDispatcher.Subscribe(FormViewEventKind.ValidationRequested, OnValidationRequested);
        //FormParameter.EventDispatcher.Subscribe(FormViewEventKind.FormDataInitialized, (sender, args) => _logger.LogInformation("EventKind: {0}", args.Args.EventKind));
        //FormParameter.OnSaveAsync = OnSaveAsync;
    }

    private async Task<RgfResult<RgfFormResult>> OnSaveAsync(RgfFormComponent component, bool refresh)
    {
        _logger.LogInformation("OnSaveAsync");
        return await component.OnSaveAsync(refresh);
    }

    private void OnValidationRequested(object? sender, IRgfEventArgs<RgfFormViewEventArgs> args)
    {
        _logger.LogInformation("EventKind: {0}, Alias: {1}", args.Args.EventKind, args.Args.Property?.Alias);
    }
}
