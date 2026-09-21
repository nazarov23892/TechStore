using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog;

/// <summary>
/// Сервис для работы с функционалом единиц измерения.
/// </summary>
public interface IUnitsService
{
    /// <summary>
    /// Возвращает список единиц измерения.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IEnumerable<UnitListDto>> GetUnitsAsync(
        CancellationToken cancellationToken = default);
}
