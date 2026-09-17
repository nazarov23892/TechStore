using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices;

/// <summary>
/// Http-сервис товаров.
/// </summary>
public interface IProductHttpService
{
    /// <summary>
    /// Выполняет запрос получения постраничного списка товаров заданной категории.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<PagedListResponseDto<ProductListItemDto>?> GetPagedListByCategoryAsync(
        int page, int perPage, string? category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполняет запрос получения товара по Id.
    /// </summary>
    /// <param name="id">Идентификатор товара.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<ProductDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default); 

    /// <summary>
    /// Выполняет запрос создания товара.
    /// </summary>
    /// <param name="createRequest">Модель запроса на создание товара.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<ProductDto?> CreateAsync(
        ProductPostDto createRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполняет запрос удаления товара.
    /// </summary>
    /// <param name="id">Идентификатор.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполняет запрос получения списка атрибутов.
    /// </summary>
    /// <param name="id">Id товара.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<IEnumerable<ProductAttributeListDto>?> GetAttributeListAsync(
        long id, CancellationToken cancellationToken = default);
}
