using TechStore.BLL.Entities;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Фабрика инструментов чтения значения атрибута.
/// </summary>
public class AttributePutValueReaderFactory : IAttributePutValueReaderFactory
{
    readonly IReadOnlyDictionary<CategoryAttributeDataTypes, IAttributePutValueReader> _readers;

    public AttributePutValueReaderFactory(
        IEnumerable<IAttributePutValueReader> readers)
    {
        _readers = readers.ToDictionary(r => r.DataType);
    }

    /// <inheritdoc/>
    public IAttributePutValueReader GetReader(CategoryAttributeDataTypes dataType)
        => _readers.TryGetValue(dataType, out var reader) 
            ? reader 
            : throw new NotSupportedException($"Data type '{dataType}' not supported.");
}

