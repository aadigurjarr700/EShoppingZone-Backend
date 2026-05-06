using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Orders> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Address as an Owned Entity
            modelBuilder.Entity<Orders>().OwnsOne(o => o.Address, a =>
            {
                a.Property(ad => ad.CustomerId).HasColumnName("Address_CustomerId");
                a.Property(ad => ad.FullName).HasColumnName("Address_FullName");
                a.Property(ad => ad.MobileNumber).HasColumnName("Address_MobileNumber");
                a.Property(ad => ad.FlatNumber).HasColumnName("Address_FlatNumber");
                a.Property(ad => ad.City).HasColumnName("Address_City");
                a.Property(ad => ad.Pincode).HasColumnName("Address_Pincode");
                a.Property(ad => ad.State).HasColumnName("Address_State");
            });

            // Configure ProductSnapshot as an Owned Entity
            modelBuilder.Entity<Orders>().OwnsOne(o => o.Product, p =>
            {
                p.Property(pr => pr.ProductId).HasColumnName("Product_ProductId");
                p.Property(pr => pr.ProductName).HasColumnName("Product_ProductName");
            });
        }
    }
}
