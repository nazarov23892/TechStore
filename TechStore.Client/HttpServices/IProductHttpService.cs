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

    /// <summary>
    /// Выполняет запрос создания товара.
    /// </summary>
    /// <param name="createRequest">Модель запроса на создание товара.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<ProductDto?> CreateAsync(
        ProductPostDto createRequest, CancellationToken cancellationToken = default);
}
