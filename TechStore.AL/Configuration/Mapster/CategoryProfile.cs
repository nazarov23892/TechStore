using Mapster;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Configuration.Mapster;

public class CategoryProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryShortDto>()
            .Map(dst => dst.Name, src => src.Key);

        config.NewConfig<Category, CategoryListDto>();
        config.NewConfig<Category, CategoryDto>();

        config.NewConfig<CategoryAttribute, CategoryAttributetDto>();
        config.NewConfig<CategoryAttribute, CategoryAttributeListDto>();
    }
}
