using MT.Web.Models;
using MT.Web.Service.Interface;
using MT.Web.Utility;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace MT.Web.Service;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenProvider _tokenProvider;
    public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
    }

    public async Task<ResponseDto?> SendAsync(RequestDto request, bool withBearer = true)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MT_API");
            HttpRequestMessage message = new();

            if (request.ContentType == SD.ContentType.MultipartFormData)
                message.Headers.Add("Accept", "*/*");
            else message.Headers.Add("Accept", SD.JsonType);

            // token
            if (withBearer)
            {
                var token = _tokenProvider.GetToken();
                message.Headers.Add("Authorization", $"Bearer {token}");
            }

            if (request.ContentType == SD.ContentType.MultipartFormData)
            {
                var content = new MultipartFormDataContent();
                foreach (var prop in request.Data.GetType().GetProperties())
                {
                    var value = prop.GetValue(request.Data);
                    if (value != null && value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                            content.Add(new StreamContent(file.OpenReadStream()), file.Name, file.FileName);
                    }
                    else content.Add(new StringContent(!string.IsNullOrWhiteSpace(value?.ToString()) ? value.ToString() : string.Empty), prop.Name);
                }
                message.Content = content;
            }
            else if (request.Data != null)
                message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, SD.JsonType);

            message.RequestUri = new Uri(request.Url);

            switch (request.ApiType)
            {
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage apiResponse = await client.SendAsync(message);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new() { IsSuccess = false, Message = "Not Found" };
                case HttpStatusCode.Forbidden:
                    return new() { IsSuccess = false, Message = "Access Denied" };
                case HttpStatusCode.Unauthorized:
                    return new() { IsSuccess = false, Message = "Unauthorized" };
                case HttpStatusCode.BadRequest:
                    return new() { IsSuccess = false, Message = "Bad Request" };
                default:
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return apiResponseDto;
            }
        }
        catch (Exception ex)
        {
            return new() { IsSuccess = false, Message = ex.Message };
        }
    }

}