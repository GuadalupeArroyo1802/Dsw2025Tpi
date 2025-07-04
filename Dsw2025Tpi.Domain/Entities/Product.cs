using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
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
        public string Sku { get; set; }
        public string Name { get; set; }
        public string? InternalCode { get; set; }
        public string? Description { get; set; }        
        public bool IsActive { get; set; }
        public Guid? IdProducto { get; set; }
        public decimal CurrentUnitPrice { get; set; }
        public int? StockQuantity { get; set; }
        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0.");
            CurrentUnitPrice = newPrice;
        }

        public void SetStock(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("El stock no puede ser negativo.");
            StockQuantity = quantity;
        }
    }
}
