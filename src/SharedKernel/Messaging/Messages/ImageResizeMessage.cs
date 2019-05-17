using System;

namespace SharedKernel.Messaging.Messages
{
    public class ImageResizeMessage
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; }
    }
}
