using System;

namespace Customers.Models
{
    public class UploadResponseModel
    {
        public Guid SelfieId { get; set; }
        public string Message { get; set; }
        public decimal Size { get; set; }
    }
}
