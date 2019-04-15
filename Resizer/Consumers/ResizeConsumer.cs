using MassTransit;
using Resizer.Utils;
using SharedKernel.Messaging.Messages;
using System;
using System.Threading.Tasks;

namespace Resizer.Consumers
{
    public class ResizeConsumer : IConsumer<ImageResizeMessage>
    {
        public Task Consume(ConsumeContext<ImageResizeMessage> context)
        {
            var file = System.IO.File.ReadAllBytes(context.Message.FilePath);
            var newFile = ImageUtils.Resize(file,40);
            System.IO.File.WriteAllBytes(context.Message.FilePath, newFile);

            var result = new ImageResizeMessageResponse()
            {
                Id = context.Message.Id,
                Size = Math.Round((decimal)newFile.Length / (1024 * 1024), 2)
            };
            context.Send(result);
            return Task.CompletedTask;
        }
    }
}
