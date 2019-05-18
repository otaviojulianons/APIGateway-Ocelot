using System;

namespace Customers.Models
{
    public class CustomerImageModel
    {
        public Guid CustomerId { get; set; }
        public Guid SelfieId { get; set; }
        public decimal Size { get; set; }
    }
}
