using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog;

/// <summary>
/// Инструмент чтения значения атрибута.
/// </summary>
public interface IAttributePutValueReader
{
    /// <summary>
    /// Тип данных, значения которого может читать.
    /// </summary>
    CategoryAttributeDataTypes DataType { get; }

    /// <summary>
    /// Читает значение из модели запроса.
    /// </summary>
    /// <param name="putValue">Модель значения атрибута для запросов.</param>
    /// <returns><see cref="AttributeValue"/></returns>
    AttributeValue Read(ProductAttributeValueListPutDto putValue);
}
