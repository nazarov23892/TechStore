using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog;

/// <summary>
/// Сервис для работы с функционалом каталога товаров.
/// </summary>
public interface IProductsService
{
    /// <summary>
    /// Возвращает модель для списка товаров каталога.
    /// </summary>
    /// <param name="pagingRequest">Модель постраничной навигации.</param>
    /// <param name="category">Категория товаров.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<PagedListResponseDto<ProductListItemDto>> GetProductPagedListAsync(
        PagingRequestDto pagingRequest, 
        string? category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает модель товара по Id.
    /// </summary>
    /// <param name="id">Идентификатор товара.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<ProductDto> GetProductById(
        long id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новый товар.
    /// </summary>
    /// <param name="request">Модель запроса на создание.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<ProductDto> CreateProductAsync(
        ProductPostDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет товар.
    /// </summary>
    /// <param name="id">Идентификатор товара.</param>
    /// <param name="value">Модель запроса на обновление.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<ProductDto> UpdateProductAsync(
        long id, ProductPutDto value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет товар.
    /// </summary>
    /// <param name="id">Идентификатор товара.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task DeleteProductAsync(
        long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список атрибутов товара.
    /// </summary>
    /// <param name="productId">Идентификатор товара.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IEnumerable<ProductAttributeListDto>> GetProductAttributesAsync(
        long productId, CancellationToken cancellationToken = default);
}
