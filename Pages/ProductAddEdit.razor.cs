using Mango.Web.Blazor.Models;
using Mango.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace Mango.Web.Blazor.Pages;

public partial class ProductAddEdit
{
    [Parameter]
    public int? Id { get; set; }

    [Inject]
    private IProductService ProductService { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private ProductDto Product { get; set; } = new();

    private bool isInEditMode;

    private string headerTitle = string.Empty;

    protected override void OnInitialized()
    {
        SetUpPageForAddEdit();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!isInEditMode)
            {
                Product = new ProductDto();
            }
            else if (isInEditMode && Id != null)
            {
                // var url = apiBaseUrl + $"products/GetProductById/{Id}";
                // Product ??= await HttpClient.GetFromJsonAsync<Product>(url);
                var response = await ProductService.GetProductByIdAsync(Id.Value);

                if (response != null && response.IsSuccess)
                {
                    Product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                }
            }
            StateHasChanged();
        }
    }

    private void SetUpPageForAddEdit()
    {
        isInEditMode = Id is not null;

        headerTitle = isInEditMode ? $"Edit Product - Id: {Id}" : "Add Product";
    }

    private async Task OnBeforInternalNavigation(LocationChangingContext context)
    {
        if (!isInEditMode && context.IsNavigationIntercepted)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", new object?[] { $"Are you sure you want to navigate away from this pages (any changes will be lost)?" }))
                context.PreventNavigation();
        }
    }

    private async Task OnSubmitForm()
    {
        ResponseDto? response;
        string operation;

        if (!isInEditMode)
        {
            operation = "Creating Product";
            response = await ProductService.CreateProductAsync(Product);
        }
        else
        {
            operation = "Updating Product";
            response = await ProductService.UpdateProductAsync(Product);
        }

        NavigationManager.NavigateTo("/products");
        if (response is not null && response.IsSuccess)
        {
            string message = $"{operation} was sussessful!";
            await ShowSuccess(message);
        }
        else
        {
            string message = $"{operation} failed!";
            await ShowError(message);
        }
    }

    private void OnCancel()
    {
        NavigationManager.NavigateTo("/products");
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
