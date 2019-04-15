using MassTransit;
using SharedKernel.Messaging.Messages;
using System;

namespace Messaging
{
    public class BusConfig
    {
        public const string QUEUE_RESIZE = "resize";
        public const string QUEUE_RESIZE_RESULT = "resize_result";

        public string Username { get; }
        public string Password { get; }
        public Uri Host { get; }

        public BusConfig(string username, string password, string host)
        {
            Username = username;
            Password = password;
            Host = new Uri(host);
            EndpointConvention.Map<ImageResizeMessage>(new Uri(Host, QUEUE_RESIZE));
            EndpointConvention.Map<ImageResizeMessageResponse>(new Uri(Host, QUEUE_RESIZE_RESULT));
        }

    }
}
