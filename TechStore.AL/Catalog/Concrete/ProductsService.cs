using Mapster;
using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.AL.Extensions;
using TechStore.BLL.Entities;
using TechStore.BLL.Exceptions;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Сервис для работы с функционалом каталога товаров.
/// </summary>
public class ProductsService : IProductsService
{
    readonly IApplicationDbContext _context;

    public ProductsService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<ProductDto> CreateProductAsync(
        ProductPostDto request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category {request.CategoryId} not found.");

        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = category,
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = product.Adapt<ProductDto>();
        return dto;
    }

    /// <inheritdoc/>
    public async Task<PagedListResponseDto<ProductListItemDto>> GetProductPagedListAsync(
        PagingRequestDto pagingRequest,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var pagingStartsZero = pagingRequest.Page - 1;
        var query = _context.Products
            .AsNoTracking()
            .Where(p => !string.IsNullOrEmpty(category) && p.Category!.Key == category);
        var totalCount = await query
            .CountAsync(cancellationToken);
        var products = await query
            .AsNoTracking()
            .Include(p => p.Category)
            .WithPaging(pagingStartsZero, pagingRequest.PerPage)
            .ToListAsync(cancellationToken);

        var dtos = products.Adapt<List<ProductListItemDto>>();
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
    public async Task<ProductDto> GetProductById(
        long id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException($"The product with Id {id} not found.");

        var dto = product.Adapt<ProductDto>();
        return dto;
    }

    /// <inheritdoc/>
    public async Task<ProductDto> UpdateProductAsync(
        long id, ProductPutDto value, CancellationToken cancellationToken = default)
    {
        var model = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new NotFoundException($"The product with Id {id} not found.");

        if (value.Name != null)
            model.Name = value.Name;
        if (value.Price != null)
            model.Price = value.Price.Value;
        if (value.Description != null)
            model.Description = value.Description;

        await _context.SaveChangesAsync(cancellationToken);

        var dto = model.Adapt<ProductDto>();
        return dto;
    }

    /// <inheritdoc/>
    public async Task DeleteProductAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException($"The product with Id {id} not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProductAttributeListDto>> GetProductAttributesAsync(
        long productId, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
                .ThenInclude(attr => attr!.Attributes)
                .ThenInclude(attr => attr.ProductValues.Where(v => v.ProductId == productId).Take(1))
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken)
            ?? throw new NotFoundException($"The product with Id {productId} not found.");

        var attributes = product.Category?.Attributes;
        var dtos = attributes.Adapt<List<ProductAttributeListDto>>();
        return dtos;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ProductAttributeListDto>> UpdateProductAttributesAsync(
        long productId, ProductAttributeValuesPutDto value, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
                .ThenInclude(attr => attr!.Attributes)
                .ThenInclude(attr => attr.ProductValues.Where(v => v.ProductId == productId).Take(1))
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken)
                ?? throw new NotFoundException($"The product with Id {productId} not found.");

        var attributesByKeyMap = product.Category!.Attributes.ToDictionary(
            a => a.Key);

        foreach (var attributePutDto in value.Values)
        {
            if (!attributesByKeyMap.TryGetValue(attributePutDto.AttributeKey, out var attribute)
                || attribute == null)
                throw new FailedPreconditionException($"Attribute key '{attributePutDto.AttributeKey}' not found.");

            if (attribute.ProductValues.Count == 0)
                attribute.ProductValues.Add(
                    new ProductAttributeValue()
                    {
                        ProductId = productId,
                    });

            var attributeValue = attribute.ProductValues.First().Value;
            attributeValue.NumericValue = null;
            attributeValue.StringValue = null;
            attributeValue.BoolValue = null;

            switch (attribute.DataType)
            {
                case CategoryAttributeDataTypes.Numeric:
                    attributeValue.NumericValue = attributePutDto.NumericValue;
                    break;
                case CategoryAttributeDataTypes.String:
                    attributeValue.StringValue = attributePutDto.StringValue;
                    break;
                case CategoryAttributeDataTypes.Boolean:
                    attributeValue.BoolValue = attributePutDto.BooleanValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(value), $"Unexpected value: '{attribute.DataType}'");
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
        var dtos = attributesByKeyMap.Values.Adapt<List<ProductAttributeListDto>>();
        return dtos;
    }
}
