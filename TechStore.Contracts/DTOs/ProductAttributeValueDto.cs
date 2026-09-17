namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель значения, содержащегося в атрибуте товара.
/// </summary>
public class ProductAttributeValueDto
{
    /// <summary>
    /// Строковое значение.
    /// </summary>
    public string? StringValue { get; set; }

    /// <summary>
    /// Числовое значение.
    /// </summary>
    public decimal? NumericValue { get; set; }

    /// <summary>
    /// Булевое значение.
    /// </summary>
    public bool? BoolValue { get; set; }
}
