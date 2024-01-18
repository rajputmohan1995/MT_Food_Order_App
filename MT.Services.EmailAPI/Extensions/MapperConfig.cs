using AutoMapper;
using MT.Services.EmailAPI.Models;
using MT.Services.EmailAPI.Models.DTO;

namespace MT.Services.EmailAPI.Extensions;

public class MapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<EmailLogger, EmailLoggerDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}