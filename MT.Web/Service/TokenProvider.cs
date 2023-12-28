using MT.Web.Service.Interface;
using MT.Web.Utility;

namespace MT.Web.Service;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void ClearToken()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
    }

    public string? GetToken()
    {
        string? token = null;
        bool hasToken = _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token) ?? false;

        return hasToken ? token : null;

    }

    public void SetToken(string token)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
    }
}