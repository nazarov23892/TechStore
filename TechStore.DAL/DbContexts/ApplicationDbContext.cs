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

    public DbSet<Unit> Units => Set<Unit>();

    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();

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
                entity.HasOne(a => a.Unit)
                    .WithMany()
                    .HasForeignKey(a => a.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
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

        modelBuilder.Entity<Unit>(
            entity =>
            {
                entity.HasIndex(u => u.Name)
                    .IsUnique();

                entity.HasData(

                    // Length.
                    new Unit()
                    {
                        Id = 1,
                        Name = "м",
                    },
                    new Unit()
                    {
                        Id = 2,
                        Name = "см",
                    },
                    new Unit()
                    {
                        Id = 3,
                        Name = "мм",
                    },

                    // Mass.
                    new Unit()
                    {
                        Id = 4,
                        Name = "г",
                    },
                    new Unit()
                    {
                        Id = 5,
                        Name = "кг",
                    },

                    // Memory.
                    new Unit()
                    {
                        Id = 6,
                        Name = "Б",
                    },
                    new Unit()
                    {
                        Id = 7,
                        Name = "КБ",
                    },
                    new Unit()
                    {
                        Id = 8,
                        Name = "МБ",
                    },

                    // Frequency.
                    new Unit()
                    {
                        Id = 9,
                        Name = "МГц",
                    },
                    new Unit()
                    {
                        Id = 10,
                        Name = "КГц",
                    },

                    // Other.
                    new Unit()
                    {
                        Id = 11,
                        Name = "Вт",
                    },
                    new Unit()
                    {
                        Id = 12,
                        Name = "шт",
                    });
            });
    }
}
