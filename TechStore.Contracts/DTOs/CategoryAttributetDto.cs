namespace TechStore.Contracts.DTOs;

/// <summary>
/// Полная модель атрибута категории товара.
/// </summary>
public class CategoryAttributetDto
{
    public long Id { get; set; }

    /// <summary>
    /// Название атрибута.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Тип данных.
    /// </summary>
    public string DataType { get; set; } = string.Empty;
}
