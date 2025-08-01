using Dsw2025Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Product : EntityBase
{
    public Product()
    {

    }
    public Product(string sku, string internalCode, string name, string descripcion, decimal price, int stock)
    {
        Sku = sku;
        InternalCode = internalCode;
        Name = name;
        Description = descripcion;
        CurrentUnitPrice = price;
        StockCuantity = stock;
        Id = Guid.NewGuid();
        IsActive = true;
    }

    public string? InternalCode { get; set; }
    public int StockCuantity { get; set; }
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public string? Name { get; set; }
    public decimal CurrentUnitPrice { get; set; }
    public bool IsActive { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }

    public int RestarStock(int cantidad)
    {
        StockCuantity -= cantidad;
        return StockCuantity;
    }
}