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
    public Task<PagedListResponseDto<ProductListItemDto>?> GetPagedListAsync(
        int page, int perPage, string? category, CancellationToken cancellationToken = default)
    {
        var uri = !string.IsNullOrEmpty(category)
            ? $"{BaseUri}/{category}"
            : BaseUri;
        var resultUri = $"{uri}?page={page}&perPage={perPage}";
        return _httpClient.GetFromJsonAsync<PagedListResponseDto<ProductListItemDto>>(
            resultUri, cancellationToken);
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
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUri}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
