using Customers.Models;
using System;
using System.Collections.Generic;

namespace Customers.Services
{
    public class CustomerService
    {
        public Dictionary<Guid, UploadResponseModel> Images { get; set; }


        public CustomerService()
        {
            Images = new Dictionary<Guid, UploadResponseModel>();
        }
    }
}
