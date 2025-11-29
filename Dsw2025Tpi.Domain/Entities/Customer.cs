using System;
using System.Collections.Generic;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Customer : EntityBase
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        public Customer(string email, string name, string phoneNumber)
            : this()
        {
            Email = email;
            Name = name;
            PhoneNumber = phoneNumber;
        }

        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }

        // Colección de Orders del cliente
        public ICollection<Order> Orders { get; set; }
    }
}
