namespace TechStore.Contracts.DTOs;

/// <summary>
/// Полная модель категории товара.
/// </summary>
public class CategoryDto
{
    public long Id { get; set; }

    /// <summary>
    /// Ключевое/техническое название (обозначение).
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемое в интерфейсе название.
    /// </summary>
    public string? DisplayName { get; set; }
}
