namespace TechStore.BLL.Entities;

/// <summary>
/// Атрибут, содержащийся в товарах конкретной категории.
/// </summary>
public class CategoryAttribute
{
    public long Id { get; set; }

    /// <summary>
    /// Название атрибута.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Идентификатор категории товара.
    /// </summary>
    public long CategoryId { get; set; }

    /// <summary>
    /// Тип данных.
    /// </summary>
    public CategoryAttributeDataTypes DataType { get; set; }

    /// <summary>
    /// Id единицы измерения.
    /// </summary>
    public long? UnitId { get; set; }

    /// <summary>
    /// Единица измерения атрибута.
    /// </summary>
    /// <remarks>Примеры: килограмм, метр.</remarks>
    public Unit? Unit { get; set; }

    /// <summary>
    /// Значения атрибута назначенные в конкретных товарах.
    /// </summary>
    public ICollection<ProductAttributeValue> ProductValues { get; set; } = [];
}
