using TechStore.BLL.Entities;

namespace TechStore.Tests.TestData;

public static class DataForSeed
{
    public static class DomainModels
    {
        public static Unit Unit1_Gb => new()
        {
            Id = 101,
            Name = "1-GB",
        };
        public static Unit Unit2_Kb => new()
        {
            Id = 102,
            Name = "2-kB",
        };
        public static Unit Unit3_Mhz => new()
        {
            Id = 103,
            Name = "3-MHz",
        };
        public static Unit Unit4_W => new()
        {
            Id = 104,
            Name = "4-W",
        };
        public static Unit Unit5_Kg => new()
        {
            Id = 105,
            Name = "5-kg",
        };
        public static Unit Unit6_Cm => new()
        {
            Id = 106,
            Name = "6-cm",
        };

        public static CategoryAttribute Attribute1_Frequency => new()
        { 
            Id = 201,
            Key = "1-Frequency",
            DataType = CategoryAttributeDataTypes.Numeric,
            UnitId = Unit3_Mhz.Id,
        };

        public static CategoryAttribute Attribute2_GpuIntegrated => new()
        {
            Id = 202,
            Key = "2-Integrated GPU",
            DataType = CategoryAttributeDataTypes.Boolean,
            UnitId = null,
        };

        public static CategoryAttribute Attribute3_Socket => new()
        {
            Id = 203,
            Key = "3-Socket",
            DataType = CategoryAttributeDataTypes.String,
            UnitId = null,
        };

        public static CategoryAttribute Attribute4_Tdp => new()
        {
            Id = 204,
            Key = "4-TDP",
            DataType = CategoryAttributeDataTypes.Numeric,
            UnitId = Unit4_W.Id,
        };


        public static IEnumerable<Unit> Units =>
            [
            Unit1_Gb,
            Unit2_Kb,
            Unit3_Mhz,
            Unit4_W,
            Unit5_Kg,
            Unit6_Cm,
        ];

        public static Category Category1_Empty=> new()
        {
            Id = 501,
            Key = "EmptyCategory1",
            DisplayName = "Empty categoty #1",
            Attributes = [],
        };

        public static Category Category2_CPUs => new()
        {
            Id = 502,
            Key = "CPUs",
            DisplayName = "CPUs for display",
            Attributes = [
                Attribute1_Frequency,
                Attribute2_GpuIntegrated,
                Attribute3_Socket, 
                Attribute4_Tdp,
            ],
        };
    }
}
