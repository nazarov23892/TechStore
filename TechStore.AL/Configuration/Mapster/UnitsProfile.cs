using Mapster;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Configuration.Mapster;

public class UnitsProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Unit, UnitDto>();
        config.NewConfig<Unit, UnitListDto>();
        config.NewConfig<Unit, UnitShortDto>();
    }
}
