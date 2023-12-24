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
    public BaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ResponseDto?> SendAsync(RequestDto request)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MT_API");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", SD.JsonType);
            //token

            message.RequestUri = new Uri(request.Url);
            if (request.Data != null)
                message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, SD.JsonType);

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