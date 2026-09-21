using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices;

/// <summary>
/// Http-сервис единиц измерения.
/// </summary>
public interface IUnitsHttpService
{
    /// <summary>
    /// Выполняет запрос получения списка единиц измерения.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<IEnumerable<UnitListDto>?> GetListAsync(
        CancellationToken cancellationToken = default);
}
