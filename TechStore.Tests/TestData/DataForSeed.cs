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

        public static CategoryAttribute Attribute5_Numeric => new()
        {
            Id = 205,
            Key = "1_Attr_Numeric",
            DataType = CategoryAttributeDataTypes.Numeric,
            UnitId = null
        };

        public static CategoryAttribute Attribute6_Boolean => new()
        {
            Id = 206,
            Key = "2_Attr_Boolean",
            DataType = CategoryAttributeDataTypes.Boolean,
            UnitId = null
        };

        public static CategoryAttribute Attribute7_String => new()
        {
            Id = 207,
            Key = "3_Attr_String",
            DataType = CategoryAttributeDataTypes.String,
            UnitId = null
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

        public static Category Category1_Empty => new()
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

        public static Category Category3_SingleNumeric => new()
        {
            Id = 503,
            Key = "SingleNumeric",
            DisplayName = "Single Numeric",
            Attributes = [
                Attribute5_Numeric,
            ],
        };

        public static Product Product1_IntelCore_i3 => new()
        {
            Id = 601,
            Name = "Intel Core i3-12100F",
            Description = "",
            CategoryId = Category2_CPUs.Id,
            Price = 7500M,
        };

        public static Product Product2_IntelCore_i5 => new()
        {
            Id = 602,
            Name = "Intel Core i5-12400F",
            Description = "",
            CategoryId = Category2_CPUs.Id,
            Price = 13500,
        };

        public static Product Product3_IntelCore_i7 => new()
        {
            Id = 603,
            Name = "Intel Core i7-12700K",
            Description = "",
            CategoryId = Category2_CPUs.Id,
            Price = 28500,
        };

        public static Product Product4_SingleNumeric => new()
        {
            Id = 605,
            Name = "SingleNumeric",
            Description = null,
            CategoryId = Category3_SingleNumeric.Id,
            Price = 605,
        };

        public static ProductAttributeValue[] Product1_IntelCore_i3_AttributeValues => [
            new()
            {
                Id = 701,
                CategoryAttributeId = Attribute1_Frequency.Id,
                ProductId = Product1_IntelCore_i3.Id,
                Value = new AttributeValue()
                {
                    NumericValue = 3.3M,
                },
            },
            new()
            {
                Id = 702,
                CategoryAttributeId = Attribute2_GpuIntegrated.Id,
                ProductId = Product1_IntelCore_i3.Id,
                Value = new AttributeValue()
                {
                    BoolValue = true,
                },
            },
            new()
            {
                Id = 703,
                CategoryAttributeId = Attribute3_Socket.Id,
                ProductId = Product1_IntelCore_i3.Id,
                Value = new AttributeValue()
                {
                    StringValue = "LGA1700",
                },
            },
            new()
            {
                Id = 704,
                CategoryAttributeId = Attribute4_Tdp.Id,
                ProductId = Product1_IntelCore_i3.Id,
                Value = new AttributeValue()
                {
                    NumericValue = 120M,
                },
            },
        ];

    }
}
