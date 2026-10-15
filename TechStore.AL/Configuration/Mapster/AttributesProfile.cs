using Mapster;
using TechStore.BLL.Entities;

namespace TechStore.AL.Configuration.Mapster;

public class AttributesProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AttributeValue, AttributeValue>();
    }
}
