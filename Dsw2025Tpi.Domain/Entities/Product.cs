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
        StockQuantity = stock;
        IsActive = true;
    }

    public string? InternalCode { get; set; }
    public int StockQuantity { get; set; }
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public string? Name { get; set; }
    public decimal CurrentUnitPrice { get; set; }
    public bool IsActive { get; set; }
    //Define un atributo de tipo Colección, con todas las OrderItems
    public ICollection<OrderItem> OrderItems { get; set; }
    public int RestarStock(int cantidad)
    {
        StockQuantity -= cantidad;
        return StockQuantity;
    }
}