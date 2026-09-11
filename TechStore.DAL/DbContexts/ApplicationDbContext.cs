using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.BLL.Entities;

namespace TechStore.DAL.DbContexts;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts)
        : base(opts)
    {

    }

    /// <inheritdoc/>
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(
            entity =>
            {
                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);
                entity.HasOne(p => p.Category)
                    .WithMany()
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        modelBuilder.Entity<Category>(
          entity =>
          {
              entity.HasIndex(c => c.Name)
              .IsUnique();
          });
    }
}
