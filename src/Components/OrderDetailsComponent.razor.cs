using Microsoft.AspNetCore.Components;
using Recrovit.RecroGridFramework.Abstraction.Contracts.Services;
using Recrovit.RecroGridFramework.Client.Blazor.Components;
using Recrovit.RecroGridFramework.Client.Events;
using RGF.Demo.Northwind.Common.Validation;

namespace RGF.Demo.Blazor.Components;

public partial class OrderDetailsComponent : ComponentBase, IDisposable
{
    [Inject]
    private IRecroDictService _recroDictService { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        EntityParameters.FormParameters.EventDispatcher.Subscribe(RgfFormEventKind.ValidationRequested, OnValidationRequestedAsync);
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
    }
}