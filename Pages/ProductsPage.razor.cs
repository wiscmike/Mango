
using Mango.Web.Blazor.Models;
using Mango.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Mango.Web.Blazor.Pages;

public partial class ProductsPage
{
    [Inject]
    private IProductService ProductService { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;


    private List<ProductDto>? Products { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await GetAllProducts();
    }

    private async Task GetAllProducts()
    {
        var response = await ProductService.GetAllProductsAsync();

        if (response != null && response.IsSuccess)
        {
            if (response.Result is not null)
            {
                // Directly cast and deserialize the result
                var products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());

                Products = products is not null ? products : [];
            }

        }
    }

    private async Task DeleteProduct(int productId)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", new object?[] { $"Are you sure you want to delete the selected Product?" }))
            return;

        var response = await ProductService.DeleteProductAsync(productId);

        if (response != null && response.IsSuccess)
        {
            await GetAllProducts();
        }
        else
        {
            await ShowError("Failed to delete Product!");
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
