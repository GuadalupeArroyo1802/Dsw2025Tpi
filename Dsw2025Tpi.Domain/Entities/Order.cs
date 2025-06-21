using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public Order(string shippingAddress, string billingAddress, DateTime date)
        {
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            IdCustomer= Guid.NewGuid();
        }
        public Guid CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }

        public Guid IdCustomer { get; set; } = Guid.NewGuid();
        public List<OrderItem> OrderItems { get; set; } = new();
        public Customer? Customer { get; set; }

    }
}