using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025Ej15Context : DbContext
{
    public Dsw2025Ej15Context(DbContextOptions<Dsw2025Ej15Context> options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Customer>(eb =>
        {
            eb.ToTable("Customer");
            eb.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();
            eb.Property(c => c.Email)
            .HasMaxLength(100)
            .IsRequired();
            eb.Property(c => c.PhoneNumber)
              .HasMaxLength(20)
              .IsRequired();
        });
        modelBuilder.Entity<Product>(eb =>
        {
            eb.ToTable("Products");
            eb.HasKey(p => p.Id);
            eb.HasIndex(p => p.Sku).IsUnique();
            eb.Property(p => p.Sku)
              .HasMaxLength(20)
              .IsRequired();
            eb.Property(p => p.InternalCode);
            eb.Property(p => p.Name)
              .HasMaxLength(50)
              .IsRequired();
            eb.Property(p => p.Description);
            });

            modelBuilder.Entity<OrderItem>(eb =>
            {
                eb.ToTable("OrderItems");
                eb.HasKey(oi => oi.Id);
                /*    eb.Property(oi => oi.Name)
                      .HasMaxLength(100)
                      .IsRequired();                            CONSULTAR
                    eb.Property(oi => oi.Description)
                      .HasMaxLength(250)
                     .IsRequired();
                  */
                eb.Property(oi => oi.UnitPrice)
                  .HasPrecision(15, 2);
                eb.Property(oi => oi.Quantity);
                eb.HasOne(oi => oi.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(oi => oi.OrderId);
                eb.HasOne(oi => oi.Product)
                  .WithMany()
                  .HasForeignKey(oi => oi.ProductId);
            });
             
              modelBuilder.Entity<Order>(eb =>
              {
                  eb.ToTable("Orders");
                  eb.HasKey(o => o.Id);
                  eb.Property(o => o.ShippingAddress)
                    .HasMaxLength(200)
                    .IsRequired();
                  eb.Property(o => o.BillingAddress)
                    .HasMaxLength(200)
                    .IsRequired();
                  eb.Property(o => o.TotalAmount)
                    .HasPrecision(15, 2);
                  eb.Property(o => o.Status);
                  eb.HasOne(o => o.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.CustomerId);
              });

    }
}
