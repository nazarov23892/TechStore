using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Инструмент чтения строкового значения атрибута.
/// </summary>
public class StringAttributePutValueReader : IAttributePutValueReader
{
    /// <inheritdoc/>
    public CategoryAttributeDataTypes DataType => CategoryAttributeDataTypes.String;

    /// <inheritdoc/>
    public AttributeValue Read(ProductAttributeValueListPutDto putValue)
        => new()
        {
            StringValue = putValue.StringValue,
        };
}
