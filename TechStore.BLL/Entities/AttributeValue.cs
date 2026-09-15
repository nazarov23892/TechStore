namespace TechStore.BLL.Entities;

/// <summary>
/// Значение атрибута.
/// </summary>
public class AttributeValue
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
