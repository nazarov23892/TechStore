using Microsoft.AspNetCore.Mvc;
using TechStore.AL.Catalog;
using TechStore.Contracts.DTOs;

namespace TechStore.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    readonly ICatalogService _catalogService;

    public ProductsController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<PagedListResponseDto<ProductListItemDto>> GetProductList(
        [FromQuery] PagingRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetProductPagedListAsync(request, cancellationToken);
        return result;
    }

    [HttpPost]
    public async Task<ProductDto> CreateProduct(
        ProductPostDto request, CancellationToken cancellationToken)
    {
        var result = await _catalogService.CreateProductAsync(request, cancellationToken);
        return result;
    }

    [HttpDelete("{id:long}")]
    public Task DeleteProduct(
        long id, CancellationToken cancellationToken)
        => _catalogService.DeleteProductAsync(id, cancellationToken);

}
