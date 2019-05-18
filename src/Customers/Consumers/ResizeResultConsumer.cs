using Customers.Services;
using MassTransit;
using SharedKernel.Messaging.Messages;
using System.Threading.Tasks;

namespace Customers.Consumers
{
    public class ResizeResultConsumer : IConsumer<ImageResizeMessageResponse>
    {
        private CustomerService _service;

        public ResizeResultConsumer(CustomerService service)
        {
            _service = service;
        }

        public Task Consume(ConsumeContext<ImageResizeMessageResponse> context)
        {
            //TODO: fix consumer
            //var image = _service.Images[context.Message.Id];
            //image.Size = context.Message.Size;
            return Task.CompletedTask;
        }
    }
}
