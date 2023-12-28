namespace MT.Web.Service.Interface;

public interface ITokenProvider
{
    string? GetToken();
    void SetToken(string token);
    void ClearToken();
}