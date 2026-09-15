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
}
