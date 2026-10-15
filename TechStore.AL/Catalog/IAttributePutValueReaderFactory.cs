using TechStore.BLL.Entities;

namespace TechStore.AL.Catalog;

/// <summary>
/// Фабрика инструментов чтения значения атрибута.
/// </summary>
public interface IAttributePutValueReaderFactory
{
    /// <summary>
    /// Возвращает инструмент для чтения значения атрибута заданного типа данных.
    /// </summary>
    /// <param name="dataType">Тип данных.</param>
    /// <returns>Экземпляр конкретного <see cref="IAttributePutValueReader"/>.</returns>
    IAttributePutValueReader GetReader(CategoryAttributeDataTypes dataType);
}
