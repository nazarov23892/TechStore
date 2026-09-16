namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель для создания атрибута категории товара.
/// </summary>
public class CategoryAttributePostDto
{
    /// <summary>
    /// Название атрибута.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Тип данных.
    /// </summary>
    public string DataType { get; set; } = string.Empty;
}
