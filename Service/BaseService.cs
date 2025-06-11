using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static Mango.Web.Utilities.StaticDetail;

namespace Mango.Web.Service;

public class BaseService : IBaseService
{
    private const string ContentType = "application/json";
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ITokenProvider tokenProvider;
    private readonly ILogger<BaseService> logger;

    public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider, ILogger<BaseService> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.tokenProvider = tokenProvider;
        this.logger = logger;
    }

    public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
    {
        try
        {
            logger.LogInformation($"Sending a {requestDto.ApiType.ToString()} http request.");

            HttpClient client = httpClientFactory.CreateClient("Mango");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", ContentType);

            // token
            if (withBearer)
            {
                var token = tokenProvider.GetToken();
                message.Headers.Add("Authorization", $"Bearer {token}");
            }

            message.RequestUri = new Uri(requestDto.Url);

            if (requestDto.Data is not null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, ContentType);
            }

            switch (requestDto.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;

                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;

                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;

                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            var apiResponse = await client.SendAsync(message);

            var apiResponseDto = await GetResponseDto(apiResponse);

            return apiResponseDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);

            return new ResponseDto() { IsSuccess = false, Message = ex.Message };
        }
    }

    private async Task<ResponseDto?> GetResponseDto(HttpResponseMessage apiResponse)
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