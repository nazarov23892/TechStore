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

        config.NewConfig<CategoryAttribute, ProductAttributeListDto>()
            .Map(dst => dst.Key, src => src.Key)
            .Map(dst => dst.DataType, src => src.DataType)
            .Map(dst => dst.Value, src => src.ProductValues.FirstOrDefault())
            .IgnoreIf(
                (src, dst) => !src.ProductValues.Any(),
                dst => dst.Value
            );
        config.NewConfig<ProductAttributeValue, ProductAttributeValueDto>()
            .Map(dst => dst, src => src.Value);

        config.NewConfig<AttributeValue, ProductAttributeValueDto>();
    }
}
