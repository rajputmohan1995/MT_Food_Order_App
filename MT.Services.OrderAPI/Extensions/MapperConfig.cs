using AutoMapper;
using MT.Services.OrderAPI.Models;
using MT.Services.OrderAPI.Models.DTO;

namespace MT.Services.OrderAPI.Extensions;

public class MapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            config.CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}