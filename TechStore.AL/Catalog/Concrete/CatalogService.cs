using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.BLL.Entities;
using TechStore.BLL.Exceptions;
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
    public async Task<ProductDto> CreateProductAsync(
        ProductPostDto request, CancellationToken cancellationToken = default)
    {
        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = new ProductDto()
        {
            Id = product.Id,
            Name = request.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
        };
        return dto;
    }

    /// <inheritdoc/>
    public async Task<PagedListResponseDto<ProductListItemDto>> GetProductPagedListAsync(
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
            p => new ProductListItemDto()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category,
            }).ToList();

        var result = new PagedListResponseDto<ProductListItemDto>()
        {
            Items = dtos,
            Page = pagingRequest.Page,
            PerPage = pagingRequest.PerPage,
            TotalCount = totalCount,
        };
        return result;
    }

    /// <inheritdoc/>
    public async Task DeleteProductAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"The product with Id {id} not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

}
