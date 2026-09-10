using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Сервис для работы с функционалом каталога товаров.
/// </summary>
public class CatalogService : ICatalogService
{
    readonly IApplicationDbContext _context;

    public CatalogService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<PagedListResponseDto<ProductCatalogListItemDto>> GetProductPagedListAsync(
        PagingRequestDto pagingRequest, CancellationToken cancellationToken = default)
    {
        var pagingStartsZero = pagingRequest.Page - 1;
        var totalCount = await _context.Products
            .CountAsync(cancellationToken);
        var products = await _context.Products
            .AsNoTracking()
            .Skip(pagingStartsZero * pagingRequest.PerPage)
            .Take(pagingRequest.PerPage)
            .ToListAsync(cancellationToken);

        var dtos = products.Select(
            p => new ProductCatalogListItemDto()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category,
            }).ToList();

        var result = new PagedListResponseDto<ProductCatalogListItemDto>()
        {
            Items = dtos,
            Page = pagingRequest.Page,
            PerPage = pagingRequest.PerPage,
            TotalCount = totalCount,
        };
        return result;
    }
}
