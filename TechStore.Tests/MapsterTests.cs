using Mapster;

namespace TechStore.Tests;

public class MapsterTests
{
    [Fact(DisplayName = "Mapster: Проверка профилей маппинга.")]
    public void ShouldProperlyMapProfiles()
    {
        var mapperConfig = new TypeAdapterConfig
        {
            RequireDestinationMemberSource = true
        };

        mapperConfig.Scan(typeof(AL.Configuration.AppConstants).Assembly);
        mapperConfig.Compile();

        Assert.True(true);
    }
}
