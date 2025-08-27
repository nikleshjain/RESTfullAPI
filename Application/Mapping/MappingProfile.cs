using AutoMapper;
using RESTfullAPI.Application.DTOs;
using RESTfullAPI.Domain.Entities;

namespace RESTfullAPI.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count));
        
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow));
        
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.ModifiedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // Item mappings
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));
        
        CreateMap<CreateItemDto, Item>();
        CreateMap<UpdateItemDto, Item>();
    }
}
