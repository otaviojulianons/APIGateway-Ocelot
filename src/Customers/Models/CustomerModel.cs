using System;

namespace Customers.Models
{
    public class CustomerModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CustomerImageModel Selfie { get; set; }

    }
}
