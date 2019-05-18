using System;

namespace SharedKernel.Messaging.Messages
{
    public class ImageResizeMessage
    {
        public Guid Id { get; set; }
        public float Percent { get; set; }
    }
}
