using Mango.Web.Blazor.Models;
using Mango.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Mango.Web.Blazor.Pages;

public partial class ProductDetail
{
    [Parameter]
    public int ProductId { get; set; } 

    [Inject]
    private IProductService ProductService { get; set; } = default!;

    private ProductDto? Product { get; set; } = new();


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var response = await ProductService.GetProductByIdAsync(ProductId);

            if (response is not null && response.IsSuccess && response.Result is not null)
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());

                Product = product;
            }
            StateHasChanged();
        }
    }
}
