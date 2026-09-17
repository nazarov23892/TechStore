using System.Net.Http.Json;
using TechStore.Contracts.Configuration;
using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices.Concrete;

/// <summary>
/// Http-сервис товаров.
/// </summary>
public class ProductHttpService : IProductHttpService
{
    readonly HttpClient _httpClient;
    const string BaseUri = SharedConstants.BaseUri.Products;

    public ProductHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public Task<PagedListResponseDto<ProductListItemDto>?> GetPagedListByCategoryAsync(
        int page, int perPage, string? category, CancellationToken cancellationToken = default)
    {
        var uri = $"{BaseUri}/category/{category}?page={page}&perPage={perPage}";
        return _httpClient.GetFromJsonAsync<PagedListResponseDto<ProductListItemDto>>(
            uri, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<ProductDto> GetByIdAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var uri = $"{BaseUri}/{id}";
        return _httpClient.GetFromJsonAsync<ProductDto>(
            uri, cancellationToken)!;
    }

    /// <inheritdoc/>
    public async Task<ProductDto?> CreateAsync(
        ProductPostDto createRequest, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUri, createRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);
        return result;
    }

    /// <inheritdoc/>
    public async Task<ProductDto> UpdateProductAsync(
        long id, ProductPutDto value, CancellationToken cancellationToken = default)
    {
        var uri = $"{BaseUri}/{id}";
        var response = await _httpClient.PutAsJsonAsync(uri, value, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);
        return result!;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUri}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc/>
    public Task<IEnumerable<ProductAttributeListDto>?> GetAttributeListAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var uri = $"{BaseUri}/{id}/attributes";
        return _httpClient.GetFromJsonAsync<IEnumerable<ProductAttributeListDto>>(uri, cancellationToken);
    }
}
