using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Services.ShoppingCartAPI.Services;

public class ProductService : IProductService
{
    private const string ProductHttp = "Product";

    private readonly IHttpClientFactory httpClientFactory;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ProductDto>?> GetProducts()
    {
        var client = httpClientFactory.CreateClient(ProductHttp);

        var response = await client.GetAsync($"/api/product");

        var apiContent = await response.Content.ReadAsStringAsync();

        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        if (resp.IsSuccess)
        {
            var results = Convert.ToString(resp.Result);

            if (!string.IsNullOrEmpty(results))
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(results);
            }
        }
        
        return new List<ProductDto>(); 
    }
}
