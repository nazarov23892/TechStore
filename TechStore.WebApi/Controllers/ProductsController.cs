using Microsoft.AspNetCore.Mvc;
using TechStore.AL.Catalog;
using TechStore.Contracts.DTOs;

namespace TechStore.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    readonly IProductsService _productsService;

    public ProductsController(IProductsService catalogService)
    {
        _productsService = catalogService;
    }

    [HttpGet("category/{category?}")]
    public async Task<ActionResult<PagedListResponseDto<ProductListItemDto>>> GetProductList(
        [FromQuery] PagingRequestDto request,
        string? category,
        CancellationToken cancellationToken)
    {
        var result = await _productsService.GetProductPagedListAsync(
            request, category, cancellationToken);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(
        long id,
        CancellationToken cancellationToken = default)
    {
        var result = await _productsService.GetProductById(id, cancellationToken);
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        ProductPostDto request, CancellationToken cancellationToken)
    {
        var result = await _productsService.CreateProductAsync(request, cancellationToken);
        return result;
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        long id,
        ProductPutDto value,
        CancellationToken cancellationToken)
    {
        var result = await _productsService.UpdateProductAsync(id, value, cancellationToken);
        return result;
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteProduct(
        long id, CancellationToken cancellationToken)
    {
        await _productsService.DeleteProductAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:long}/attributes")]
    public async Task<ActionResult<IEnumerable<ProductAttributeListDto>>> GetProductAttributes(
        long id, CancellationToken cancellationToken)
    {
        var result = await _productsService.GetProductAttributesAsync(id, cancellationToken);
        return result.ToList();
    }

    [HttpPut("{id:long}/attributes")]
    public async Task<ActionResult<ProductDto>> UpdateProductAttributes(
        long id,
        ProductAttributeValuesPutDto value,
        CancellationToken cancellationToken)
    {
        var result = await _productsService.UpdateProductAsync(id, value, cancellationToken);
        return result;
    }
}
