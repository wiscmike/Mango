using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

public class AuthService : IAuthService
{
    private readonly string authUrl = StaticDetail.AuthAPIBase + "/api/Auth";
    private readonly IBaseService baseService;

    public AuthService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public async Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequest)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.POST,
            Data = registrationRequest,
            Url = authUrl + "/register"
        }, withBearer: false);
    }

    public async Task<ResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.POST,
            Data = loginRequest,
            Url = authUrl + "/login"
        }, withBearer: false); ;
    }

    public async Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto registrationRequest)
    {
        return await baseService.SendAsync(new RequestDto
        {
            ApiType = StaticDetail.ApiType.POST,
            Data = registrationRequest,
            Url = authUrl + "/assignrole"
        });
    }
}
