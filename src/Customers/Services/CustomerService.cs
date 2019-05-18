using Customers.Models;
using MassTransit;
using Microsoft.Extensions.Configuration;
using RestSharp;
using SharedKernel.FileServer;
using SharedKernel.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.IO;

namespace Customers.Services
{
    public class CustomerService
    {
        private Dictionary<Guid,CustomerModel> _customers { get; set; }

        private IBusControl _bus;

        private FileServerService _fileServerService;

        public CustomerService(IBusControl bus, IConfiguration configuration)
        {
            _bus = bus;
            _fileServerService = new FileServerService(configuration.GetValue<string>("FileServerUrl"));
            _customers = new Dictionary<Guid,CustomerModel>()
            {
                {
                    new Guid("ceb58785-56f6-4ef6-8591-577eb00b42d5"),
                    new CustomerModel()
                    {
                        Id = new Guid("ceb58785-56f6-4ef6-8591-577eb00b42d5"),
                        Name = "Otávio Juliano Nielsen Silva",
                    }
                }
            };

        }

        public byte[] DonloadSelfie(Guid id)
        {
            var customer = _customers.GetValueOrDefault(id);
            if (customer == null)
                throw new Exception("Customer not found.");

            if (customer.Selfie == null)
                throw new Exception("Selfie not found.");

            return _fileServerService.Download(customer.Selfie.SelfieId);
        }

        public void UploadSelfie(Guid id, Stream file)
        {
            var customer = _customers.GetValueOrDefault(id);
            if (customer == null)
                throw new Exception("Customer not found.");

            var selfieId = _fileServerService.Upload(file);

            var selfie = new CustomerImageModel()
            {
                CustomerId = id,
                SelfieId = selfieId,
                Size = Math.Round((decimal)file.Length / (1024 * 1024), 2)
            };

            customer.Selfie = selfie;

            _bus.Send(new ImageResizeMessage()
            {
                Id = selfieId,
                Percent = 10
            });
        }

        public IEnumerable<CustomerModel> GetCustomers() => _customers.Values;

        public CustomerModel GetCustomer(Guid id) => _customers.GetValueOrDefault(id, null);

    }
}
