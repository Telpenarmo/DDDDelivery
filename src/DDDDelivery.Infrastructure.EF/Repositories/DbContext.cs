using DDDDelivery.Domain;
using DDDDelivery.Infrastructure.EF.Repositories.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DDDDelivery.Infrastructure.EF.Repositories;

public class OrdersContext : DbContext
{
    public string DbPath { get; init; }

    public OrdersContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "ddddelivery.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var ordersBuilder = modelBuilder.Entity<OrderDto>();
        ordersBuilder.ToTable("Orders");

        ordersBuilder.OwnsMany(o => o.OrderLines, ol =>
        {
            ol.ToTable("OrderLines");
            ol.HasKey(ol => new { ol.OrderId, ol.ProductId });
            ol.HasOne(typeof(ProductDto)).WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Restrict);
        });

        var productBuilder = modelBuilder.Entity<ProductDto>();
        productBuilder.ToTable("Products");

        var customerBuilder = modelBuilder.Entity<CustomerDto>();
        customerBuilder.ToTable("Customers");
        customerBuilder.OwnsOne(c => c.PrimaryContactInfo);
        customerBuilder.OwnsOne(c => c.SecondaryContactInfo);
    }
}