using MassTransit;
using Microsoft.Extensions.Configuration;
using Resizer.Utils;
using SharedKernel.FileServer;
using SharedKernel.Messaging.Messages;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Resizer.Consumers
{
    public class ResizeConsumer : IConsumer<ImageResizeMessage>
    {
        private FileServerService _fileServerService;

        public ResizeConsumer(IConfiguration configuration)
        {
            _fileServerService = new FileServerService(configuration.GetValue<string>("FileServerUrl"));
        }

        public Task Consume(ConsumeContext<ImageResizeMessage> context)
        {
            var id = context.Message.Id;
            var height = context.Message.Height;
            var width = context.Message.Width;

            var file = _fileServerService.Download(id);
            var newFile = ImageUtils.Resize(file, height, width);
            _fileServerService.Update(new MemoryStream(newFile), id);

            var result = new ImageResizeMessageResponse()
            {
                Id = id,
                Size = Math.Round((decimal)newFile.Length / (1024 * 1024), 2)
            };
            context.Send(result);

            return Task.CompletedTask;
        }
    }
}
