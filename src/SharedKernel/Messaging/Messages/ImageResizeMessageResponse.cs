using System;

namespace SharedKernel.Messaging.Messages
{
    public class ImageResizeMessageResponse
    {
        public Guid Id { get; set; }
        public decimal Size { get; set; }
    }
}
