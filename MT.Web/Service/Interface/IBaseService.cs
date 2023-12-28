using MT.Web.Models;

namespace MT.Web.Service.Interface;

public interface IBaseService
{
    Task<ResponseDto?> SendAsync(RequestDto request, bool withBearer = true);
}