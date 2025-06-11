using Mango.Web.Blazor.Models;
using Mango.Web.Blazor.Utilities;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace Mango.Web.Blazor.Services;

public class CouponService : ICouponService
{
    private readonly string CouponUrl = "api/coupon";

    private readonly HttpClient _httpClient;

    private readonly ILogger<CouponService> _logger;

    public CouponService(IHttpClientFactory httpClientFactory, ILogger<CouponService> logger)
    {
        // Create the named HttpClient
        _httpClient = httpClientFactory.CreateClient(StaticUtility.CouponAPIName);
        _logger = logger;
    }

    public async Task<ResponseDto?> GetAllCouponsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<ResponseDto>(CouponUrl);

        return response;
    }

    public async Task<ResponseDto?> GetCouponByCodeAsync(string couponCode)
    {
        var response = await _httpClient.GetFromJsonAsync<ResponseDto>($"{CouponUrl}/GetByCode/{couponCode}");

        return response;
    }

    public async Task<ResponseDto?> GetCouponByIdAsync(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<ResponseDto>($"{CouponUrl}/{id}");

        return response;
    }

    public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
    {
        try
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            // token - later
            var url = _httpClient.BaseAddress;
            message.RequestUri = new Uri(url + CouponUrl);

            // Serialize the couponDto and set the request content
            message.Content = new StringContent(JsonConvert.SerializeObject(couponDto), Encoding.UTF8, "application/json");

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

    public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
    {
        try
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            // token - later
            var url = _httpClient.BaseAddress;
            message.RequestUri = new Uri(url + CouponUrl);

            // Serialize the couponDto and set the request content
            message.Content = new StringContent(JsonConvert.SerializeObject(couponDto), Encoding.UTF8, "application/json");

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

    public async Task<ResponseDto> DeleteCouponAsync(int id)
    {
        HttpRequestMessage message = new();
        message.Headers.Add("Accept", "application/json");

        // token - later
        var url = _httpClient.BaseAddress;
        message.RequestUri = new Uri(url + CouponUrl + $"/{id}");

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
