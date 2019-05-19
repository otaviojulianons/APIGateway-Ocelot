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

        public byte[] DonloadAvatar(Guid id)
        {
            var customer = _customers.GetValueOrDefault(id);
            if (customer == null)
                throw new Exception("Customer not found.");

            if (customer.Avatar == null)
                throw new Exception("Avatar not found.");

            return _fileServerService.Download(customer.Avatar.AvatarId);
        }

        public void UploadAvatar(Guid id, Stream file)
        {
            var customer = _customers.GetValueOrDefault(id);
            if (customer == null)
                throw new Exception("Customer not found.");

            var avatarId = _fileServerService.Upload(file);

            var avatar = new CustomerImageModel()
            {
                CustomerId = id,
                AvatarId = avatarId,
                Size = Math.Round((decimal)file.Length / (1024 * 1024), 2)
            };

            customer.Avatar = avatar;

            _bus.Send(new ImageResizeMessage()
            {
                Id = avatarId,
                Height = 64,
                Width = 64
            });
        }

        public IEnumerable<CustomerModel> GetCustomers() => _customers.Values;

        public CustomerModel GetCustomer(Guid id) => _customers.GetValueOrDefault(id, null);

    }
}
