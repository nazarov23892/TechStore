using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog;

/// <summary>
/// Сервис для работы с функционалом каталога товаров.
/// </summary>
public interface ICatalogService
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
    /// Создает новый товар.
    /// </summary>
    /// <param name="request">Модель запроса на создание.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<ProductDto> CreateProductAsync(
        ProductPostDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет товар.
    /// </summary>
    /// <param name="id">Идентификатор товара.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task DeleteProductAsync(
        long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ответ со списком моделей категорий.
    /// </summary>
    /// <param name="pagingRequest">Модель постраничной навигации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<PagedListResponseDto<CategoryListDto>> GetCategoriesAsync(
        PagingRequestDto pagingRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает категорию по Id.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<CategoryDto> GetCategyByIdAsync(
        long categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет категорию.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="value">Модель запроса на обновление..</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<CategoryDto> UpdateCategoryAsync(
        long categoryId,
        CategoryPutDto value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список атрибутов категории товаров.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IEnumerable<CategoryAttributeListDto>> GetCategyAtrributesAsync(
        long categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новую категорию товаров.
    /// </summary>
    /// <param name="request">Модель запроса на создание.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<CategoryDto> CreateCategoryAsync(
        CategoryPostDto request, CancellationToken cancellationToken = default);
}
