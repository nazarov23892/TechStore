using TechStore.BLL.Entities;

namespace TechStore.Tests.TestData;

public static class DataForSeed
{
    public static class DomainModels
    {
        public static Unit Unit1_Gb => new Unit()
        {
            Name = "1-GB",
        };
        public static Unit Unit2_Kb => new Unit()
        {
            Name = "2-kB",
        };
        public static Unit Unit3_Mhz => new Unit()
        {
            Name = "3-MHz",
        };

        public static Category Category1_Processors => new()
        {
            Id = 501,
            Key = "processors",
            DisplayName = "CPUs",
            Attributes = [],
        };

        public static IEnumerable<Unit> Units =>
            [
            Unit1_Gb,
            Unit2_Kb,
            Unit3_Mhz,
            new Unit()
            {
                Name = "4-kg",
            },
            new Unit()
            {
                Name = "5-cm",
            },
            new Unit()
            {
                Name = "6-W",
            }];
    }
}
