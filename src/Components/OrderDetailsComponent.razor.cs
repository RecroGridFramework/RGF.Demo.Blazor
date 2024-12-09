using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
using Recrovit.RecroGridFramework.Abstraction.Models;
using Recrovit.RecroGridFramework.Client.Blazor.Components;
using Recrovit.RecroGridFramework.Client.Blazor.Parameters;
using Recrovit.RecroGridFramework.Client.Events;
using RGF.Demo.Northwind.Common.Validation;
using System.Globalization;

namespace RGF.Demo.Blazor.Components;

public partial class OrderDetailsComponent : ComponentBase, IDisposable, IAsyncDisposable
{
    [Inject]
    private IRecroDictService _recroDictService { get; set; } = null!;

    [Inject]
    private IJSRuntime _jsRuntime { get; set; } = null!;

    private IJSObjectReference? _module;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        EntityParameters.FormParameters.EventDispatcher.Subscribe(RgfFormEventKind.ValidationRequested, OnValidationRequestedAsync);
        EntityParameters.FormParameters.EventDispatcher.Subscribe(RgfFormEventKind.Rendered, OnAfterRenderForm);
        EntityParameters.FormParameters.FormItemTemplate = CreateFormItemTemplate;
    }

    private void OnAfterRenderForm(IRgfEventArgs<RgfFormEventArgs> args)
    {
        UpdateTooltip();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./Components/OrderDetailsComponent.razor.js");
        }
    }

    private RenderFragment CreateFormItemTemplate(RgfFormItemParameters param)
    {
        if (param.Property.Alias.Equals("Discount", StringComparison.OrdinalIgnoreCase))
        {
            param.Property.PropertyDesc.FormType = PropertyFormType.Custom;
            if (param.ItemData.DoubleValue != null)
            {
                param.ItemData.Value = ((double)param.ItemData.DoubleValue).ToString(new CultureInfo("en"));
            }
            return (builder) =>
            {
                //builder.AddMarkupContent(0, $"<div>Custom {param.Property.PropertyDesc.ColTitle}</div>");
                builder.AddContent(1, param.BaseFormComponent.FormItemTemplate(param));
                builder.AddContent(2, CustomFormItem(param));
            };
        }
        return param.BaseFormComponent.FormItemTemplate(param);
    }

    private async Task OnValidationRequestedAsync(IRgfEventArgs<RgfFormEventArgs> arg)
    {
        var form = arg.Args.BaseFormComponent as RgfFormComponent;
        if (form?.FormValidation == null || form.FormData.DataRec == null)
        {
            return;
        }
        //if (arg.Args.FieldId?.FieldName.Equals("Discount", StringComparison.OrdinalIgnoreCase) == true)
        //{
        //    var discount = form.FormData.DataRec.GetItemData("discount").DoubleValue;
        //    if (discount != 0 && (discount < 0.01 || discount > 0.9))
        //    {
        //        form.FormValidation.AddFieldErrorMessage("discount", "Discount must be between 0.01 and 0.9");
        //        form.FormValidation.AddFormErrorMessage("Client-side validation");
        //    }
        //}

        await OrderDetailValidation.ValidateAsync(form.FormData.DataRec, form.FormValidation, _recroDictService, arg.Args.FieldId?.FieldName);
    }

    public void Dispose()
    {
        EntityParameters.FormParameters.EventDispatcher.Unsubscribe(RgfFormEventKind.ValidationRequested, OnValidationRequestedAsync);
        EntityParameters.FormParameters.EventDispatcher.Unsubscribe(RgfFormEventKind.Rendered, OnAfterRenderForm);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.DisposeAsync();
        }
    }
}