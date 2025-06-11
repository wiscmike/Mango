
using Mango.Web.Blazor.Models;
using Mango.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Mango.Web.Blazor.Pages;

public partial class CouponsPage
{
    [Inject]
    private ICouponService CouponService { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;


    private List<CouponDto>? Coupons { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await GetAllCoupons();
    }

    private async Task GetAllCoupons()
    {
        var response = await CouponService.GetAllCouponsAsync();

        if (response != null && response.IsSuccess)
        {
            if (response.Result is not null)
            {
                // Directly cast and deserialize the result
                var coupons = JsonConvert.DeserializeObject<List<CouponDto>>(response.Result.ToString());

                Coupons = coupons is not null ? coupons : []; //.AsQueryable();
            }

        }
    }

    private async Task DeleteCoupon(int couponId)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", new object?[] { $"Are you sure you want to delete the Coupon?" }))
            return;

        var response = await CouponService.DeleteCouponAsync(couponId);

        if (response != null && response.IsSuccess)
        {
            await GetAllCoupons();
        }
        else
        {
            await ShowError("Failed to delete Coupon!");
        }
    }

    private async Task ShowSuccess(string message)
    {
        await JSRuntime.InvokeVoidAsync("toastrInterop.showSuccess", message, "Success");
    }

    private async Task ShowError(string message)
    {
        await JSRuntime.InvokeVoidAsync("toastrInterop.showError", message, "Error");
    }
}
