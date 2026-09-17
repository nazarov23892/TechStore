using Mapster;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Configuration.Mapster;

public class ProductProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .IgnoreIf((src, dst) => src.Category == null, dst => dst.Category!)
            .AfterMapping(
                (src, dst) => dst.Category ??= new CategoryShortDto() 
                { 
                    Id = src.CategoryId, 
                });

        config.NewConfig<Product, ProductListItemDto>()
            .Map(dst => dst.Category, src => src.Category!.Key)
            .IgnoreIf((src, dst) => src.Category == null, dst => dst.Category);

        config.NewConfig<ProductAttribute, ProductAttributeListDto>()
            .Map(dst => dst.Key, src => src.CategoryAttribute!.Key)
            .Map(dst => dst.DataType, src => src.CategoryAttribute!.DataType)
            .IgnoreIf(
                (src, dst) => src.CategoryAttribute == null,
                dst => dst.Key,
                dst => dst.DataType
            );

        config.NewConfig<AttributeValue, ProductAttributeValueDto>();
    }
}
