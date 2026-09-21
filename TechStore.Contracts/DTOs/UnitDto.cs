namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель единицы измерения.
/// </summary>
public class UnitDto
{
    public long Id { get; set; }

    /// <summary>
    /// Обозначение.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
