namespace TechStore.Contracts.DTOs;

/// <summary>
/// Списочная модель категории товара.
/// </summary>
public class CategoryListDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
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
