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

    [HttpGet("{category?}")]
    public async Task<ActionResult<PagedListResponseDto<ProductListItemDto>>> GetProductList(
        [FromQuery] PagingRequestDto request,
        string? category,
        CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetProductPagedListAsync(
            request, category, cancellationToken);
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        ProductPostDto request, CancellationToken cancellationToken)
    {
        var result = await _catalogService.CreateProductAsync(request, cancellationToken);
        return result;
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteProduct(
        long id, CancellationToken cancellationToken)
    {
        await _catalogService.DeleteProductAsync(id, cancellationToken);
        return NoContent();
    }
}
