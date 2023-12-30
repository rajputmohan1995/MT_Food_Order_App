using AutoMapper;
using MT.Services.ProductAPI.Models;
using MT.Services.ProductAPI.Models.DTO;

namespace MT.Services.ProductAPI.Extensions;

public class MapperConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<Product, ProductDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}