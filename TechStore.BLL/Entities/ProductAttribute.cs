namespace TechStore.BLL.Entities;

/// <summary>
/// Атрибут товара.
/// </summary>
public class ProductAttribute
{
    public long Id { get; set; }

    /// <summary>
    /// Идентификатор товара.
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// Идентификатор атрибута категории товара.
    /// </summary>
    public long CategoryAttributeId { get; set; }

    /// <summary>
    /// Атрибут категории товара.
    /// </summary>
    public CategoryAttribute? CategoryAttribute { get; set; }

    /// <summary>
    /// Значение.
    /// </summary>
    public AttributeValue Value { get; set; } = new();
}
