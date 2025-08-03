using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext : DbContext
{

    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var dbProduct = modelBuilder.Entity<Product>().ToTable("Products");

        dbProduct.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);
        dbProduct.HasIndex(p => p.Sku)
            .IsUnique();
        dbProduct.Property(p => p.InternalCode)
            .IsRequired()
            .HasMaxLength(50);
        dbProduct.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();
        dbProduct.Property(p => p.Description)
            .HasMaxLength(300);
        dbProduct.Property(p => p.CurrentUnitPrice)
            .HasPrecision(15, 2)
            .IsRequired();
        dbProduct.Property(p => p.StockQuantity)
            .HasMaxLength(10);

        var dbCustomer = modelBuilder.Entity<Customer>().ToTable("Customers");
        dbCustomer.Property(c => c.Name)
            .HasMaxLength(50)
            .IsRequired();
        dbCustomer.Property(c => c.Email)
            .HasMaxLength(100)
            .IsRequired();
        dbCustomer.Property(c => c.PhoneNumber)
            .HasMaxLength(10)
            .IsRequired();

        var dbOrder = modelBuilder.Entity<Order>().ToTable("Orders");
        dbOrder.Property(o => o.ShippingAddress)
            .HasMaxLength(200)
            .IsRequired();
        dbOrder.Property(o => o.BillingAddress)
            .HasMaxLength(200)
            .IsRequired();
        dbOrder.Property(o => o.Notes)
            .HasMaxLength(500);
        dbOrder.Ignore(o => o.TotalAmount);


        var dbOrderItem = modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        dbOrderItem.Property(oi => oi.UnitPrice)
    .HasPrecision(15, 2)
    .IsRequired();
        dbOrderItem.Property(oi => oi.Quantity)
            .IsRequired();
        dbOrderItem.Ignore(oi => oi.Subtotal);


    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}