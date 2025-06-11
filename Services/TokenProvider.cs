

using Mango.Web.Blazor.Utilities;
using Microsoft.AspNetCore.Http;

namespace Mango.Web.Blazor.Services;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public void SetToken(string token)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Append(StaticUtility.TokenCookie, token);
    }

    public string? GetToken()
    {
        string? token = null;

        var hasToken = httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticUtility.TokenCookie, out token);

        return hasToken is true ? token : null;
    }

    public void ClearToken()
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(StaticUtility.TokenCookie);
    }
}
