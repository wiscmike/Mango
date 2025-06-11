using Mango.Web.Blazor.Models;
using Mango.Web.Blazor.Utilities;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace Mango.Web.Blazor.Services;

public class ProductService : IProductService
{
    private readonly string ProductUrl = "api/product";

    private readonly HttpClient _httpClient;

    private readonly ILogger<ProductService> _logger;

    public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
    {
        // Create the named HttpClient
        _httpClient = httpClientFactory.CreateClient(StaticUtility.ProductAPIName);
        _logger = logger;
    }

    public async Task<ResponseDto?> GetAllProductsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<ResponseDto>(ProductUrl);

        return response;
    }

    public async Task<ResponseDto?> GetProductByNameAsync(string productName)
    {
        var response = await _httpClient.GetFromJsonAsync<ResponseDto>($"{ProductUrl}/GetByName/{productName}");

        return response;
    }

    public async Task<ResponseDto?> GetProductByIdAsync(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<ResponseDto>($"{ProductUrl}/{id}");

        return response;
    }

    public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
    {
        try
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            // token - later
            var url = _httpClient.BaseAddress;
            message.RequestUri = new Uri(url + ProductUrl);

            // Serialize the couponDto and set the request content
            message.Content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");

            message.Method = HttpMethod.Post;

            var apiResponse = await _httpClient.SendAsync(message);

            var apiResponseDto = await GetResponseDto(apiResponse);

            return apiResponseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return new ResponseDto() { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
    {
        try
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            // token - later
            var url = _httpClient.BaseAddress;
            message.RequestUri = new Uri(url + ProductUrl);

            // Serialize the couponDto and set the request content
            message.Content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");

            message.Method = HttpMethod.Put;

            var apiResponse = await _httpClient.SendAsync(message);

            var apiResponseDto = await GetResponseDto(apiResponse);

            return apiResponseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);

            return new ResponseDto() { IsSuccess = false, Message = ex.Message };
        }
    }

    public async Task<ResponseDto> DeleteProductAsync(int id)
    {
        HttpRequestMessage message = new();
        message.Headers.Add("Accept", "application/json");

        // token - later
        var url = _httpClient.BaseAddress;
        message.RequestUri = new Uri(url + ProductUrl + $"/{id}");

        message.Method = HttpMethod.Delete;

        var apiResponse = await _httpClient.SendAsync(message);

        var apiResponseDto = await GetResponseDto(apiResponse);

        return apiResponseDto;
    }

    private async Task<ResponseDto> GetResponseDto(HttpResponseMessage apiResponse)
    {
        switch (apiResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return new() { IsSuccess = false, Message = "Not Found" };

            case HttpStatusCode.Forbidden:
                return new() { IsSuccess = false, Message = "Access Denied" };

            case HttpStatusCode.Unauthorized:
                return new() { IsSuccess = false, Message = "Unauthorized" };

            case HttpStatusCode.InternalServerError:
                return new() { IsSuccess = false, Message = "Internal Server Error" };

            default:
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                return apiResponseDto;
        }
    }
}
