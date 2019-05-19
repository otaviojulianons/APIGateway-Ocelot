using System;

namespace SharedKernel.Messaging.Messages
{
    public class ImageResizeMessage
    {
        public Guid Id { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
