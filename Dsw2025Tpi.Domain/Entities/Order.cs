namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public Order()
        {

        }
        public Order(Guid customerId, string shippingAddress, string billingAddress, List<OrderItem> orderItems, DateTime createdAt, OrderStatus status)
        {
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            OrderItems = orderItems;
            Id = Guid.NewGuid();
            CreatedAt = createdAt;
            Status = status;
        }
        public Order(Guid customerId, string shippingAddress, string billingAddress, string notes, List<OrderItem> orderItems, DateTime createdAt, OrderStatus status)
        {
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            Notes = notes;
            OrderItems = orderItems;
            Id = Guid.NewGuid();
            CreatedAt = createdAt;
            Status = status;
        }
        public OrderStatus Status { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount => OrderItems.Sum(item => item.Subtotal);

        //Foreign Key de Customer
        public Guid CustomerId { get; set; }

        //Colección de Ítems de la Order
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        //Cambio de estado de la orden
        public void ChangeStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }
    }
}