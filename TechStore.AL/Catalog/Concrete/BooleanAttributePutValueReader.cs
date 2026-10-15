using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Инструмент чтения boolean-значения атрибута.
/// </summary>
public class BooleanAttributePutValueReader : IAttributePutValueReader
{
    /// <inheritdoc/>
    public CategoryAttributeDataTypes DataType => CategoryAttributeDataTypes.Boolean;

    /// <inheritdoc/>
    public AttributeValue Read(ProductAttributeValueListPutDto putValue)
        => new()
        {
            BoolValue = putValue.BooleanValue,
        };
}
