using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Инструмент чтения числового значения атрибута.
/// </summary>
public class NumericAttributePutValueReader : IAttributePutValueReader
{
    /// <inheritdoc/>
    public CategoryAttributeDataTypes DataType => CategoryAttributeDataTypes.Numeric;

    /// <inheritdoc/>
    public AttributeValue Read(ProductAttributeValueListPutDto putValue)
        => new()
        {
            NumericValue = putValue.NumericValue,
        };
}
