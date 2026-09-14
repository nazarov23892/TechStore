namespace TechStore.BLL.Entities;

/// <summary>
/// Категория товаров.
/// </summary>
public class Category
{
    public long Id { get; set; }
    
    /// <summary>
    /// Ключевое/техническое название (обозначение).
    /// </summary>
    /// <remarks>Используетя в URL.</remarks>
    public required string Key { get; set; }

    /// <summary>
    /// Отображаемое в интерфейсе название.
    /// </summary>
    public string? DisplayName { get; set; }
}