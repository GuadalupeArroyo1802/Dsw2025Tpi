namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem()
        {

        }
        public OrderItem(Guid productId, Product product, int quantity, decimal currentUnitPrice)
        {
            ProductId = productId;
            Product = product;
            Quantity = quantity;
            UnitPrice = currentUnitPrice;
        }

        //Foreign Key Product
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        //Foreign Key Order
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        //Calcula el subtotal
        public decimal Subtotal => UnitPrice * Quantity;
    }
}