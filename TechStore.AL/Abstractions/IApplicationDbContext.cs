using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Entities;

namespace TechStore.AL.Abstractions;

/// <summary>
/// EF контекст доступа к БД.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
