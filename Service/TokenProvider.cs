using Mango.Web.Service.IService;
using Mango.Web.Utilities;

namespace Mango.Web.Service;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public void SetToken(string token)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Append(StaticDetail.TokenCookie, token);
    }

    public string? GetToken()
    {
        string? token = null;
        
        var hasToken = httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetail.TokenCookie, out token);
        
        return hasToken is true ? token : null;    
    }

    public void ClearToken()
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetail.TokenCookie);
    }
}
