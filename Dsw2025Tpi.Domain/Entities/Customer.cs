using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Customer : EntityBase
    {

        public Customer(string email, string name, string phoneNumber)
        {
            Email = email;
            Name = name;
            PhoneNumber = phoneNumber;
            Id = Guid.NewGuid();
        }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }

        //Orders
        public ICollection<Order> Orders { get; set; }


    }
}