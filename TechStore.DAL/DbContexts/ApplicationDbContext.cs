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
              entity.HasIndex(c => c.Key)
                .IsUnique();

              entity.HasMany(c => c.Attributes)
                  .WithOne()
                  .HasForeignKey(a => a.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
          });

        modelBuilder.Entity<CategoryAttribute>(
            entity =>
            {
                entity.HasIndex(a => new { a.CategoryId, a.Key })
                    .IsUnique();
                entity.Property(a => a.DataType)
                    .HasDefaultValue(CategoryAttributeDataTypes.String)
                    .HasConversion<string>();
            });

        modelBuilder.Entity<ProductAttributeValue>(
            entity =>
            {
                entity.HasOne(pa => pa.CategoryAttribute)
                    .WithMany(ca => ca.ProductValues)
                    .HasForeignKey(pa => pa.CategoryAttributeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(p => p.Product)
                    .WithMany()
                    .HasForeignKey(pa => pa.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.OwnsOne(
                    pa => pa.Value,
                    builder =>
                    {
                        builder.WithOwner();
                        builder.Property(p => p.NumericValue)
                            .HasPrecision(10, 2);
                    });
            });
    }
}
