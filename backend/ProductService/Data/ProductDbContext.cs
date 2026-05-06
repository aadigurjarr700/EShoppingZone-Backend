using Microsoft.EntityFrameworkCore;
using ProductService.Entities;
using System.Collections.Generic;
using System.Text.Json;

namespace ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure value conversions for complex types using System.Text.Json
            modelBuilder.Entity<Product>()
                .Property(p => p.Rating)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<int, double>>(v, (JsonSerializerOptions)null) ?? new Dictionary<int, double>()
                );

            modelBuilder.Entity<Product>()
                .Property(p => p.Review)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<int, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<int, string>()
                );

            modelBuilder.Entity<Product>()
                .Property(p => p.Image)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<IList<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                );

            modelBuilder.Entity<Product>()
                .Property(p => p.Specification)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>()
                );
        }
    }
}
