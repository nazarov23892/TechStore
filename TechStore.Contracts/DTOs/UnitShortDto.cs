namespace TechStore.Contracts.DTOs;

/// <summary>
/// Краткая модель единицы измерения.
/// </summary>
public class UnitShortDto
{
    public long Id { get; set; }

    /// <summary>
    /// Обозначение.
    /// </summary>
    public required string Name { get; set; }
}
