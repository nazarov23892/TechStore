namespace TechStore.Contracts.DTOs;

/// <summary>
/// Краткая модель категории товара.
/// </summary>
public class CategoryShortDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Название.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемое в интерфейсе название.
    /// </summary>
    public string? DisplayName { get; set; }
}
