using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
     public class Order : EntityBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string? ShippingAddress { get; set; } 
        public string? BillingAddress { get; set; } 
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();

        public Order(string shippingAddress, string billingAddress, DateTime date)
        {
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            CustomerId = Guid.NewGuid();
           
        } 
    }
}
