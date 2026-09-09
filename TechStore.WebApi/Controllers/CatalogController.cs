using Microsoft.AspNetCore.Mvc;
using TechStore.AL.Catalog;
using TechStore.Contracts.DTOs;

namespace TechStore.WebApi.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController : Controller
{
    readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<PagedListResponseDto<ProductCatalogListItemDto>> GetProductList(
        [FromQuery] PagingRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetProductPagedListAsync(request, cancellationToken);
        return result;
    }
}
