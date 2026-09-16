using Mapster;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Configuration.Mapster;

public class CategoryProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryShortDto>();

        config.NewConfig<Category, CategoryListDto>();
        config.NewConfig<Category, CategoryDto>();
    }
}
