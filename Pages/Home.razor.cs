
using Mango.Web.Blazor.Models;
using Mango.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Mango.Web.Blazor.Pages;

public partial class Home
{
    [Inject]
    private IProductService ProductService { get; set; } = default!;

    private IEnumerable<ProductDto> Products { get; set; } = [];

    protected async override Task OnInitializedAsync()
    {
        await GetProducts();
    }

    private async Task GetProducts()
    {
        var response = await ProductService.GetAllProductsAsync();

        if (response is not null && response.IsSuccess && response.Result is not null)
        {
            var products = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());

            Products = products is not null ? products : [];
        }
    }
}
