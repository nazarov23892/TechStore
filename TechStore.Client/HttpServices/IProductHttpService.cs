using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices;

/// <summary>
/// Http-сервис товаров.
/// </summary>
public interface IProductHttpService
{
    /// <summary>
    /// Выполняет запрос получения постраничного списка товаров.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<PagedListResponseDto<ProductListItemDto>?> GetPagedListAsync(
        int page, int perPage, CancellationToken cancellationToken = default);
}
