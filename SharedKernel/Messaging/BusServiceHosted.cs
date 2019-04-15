using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public class BusServiceHosted : IHostedService
    {
        private readonly IBusControl _busControl;

        public BusServiceHosted(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("--- Starting bus control ---");
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("--- Stoping bus control ---");
            return _busControl.StopAsync(cancellationToken);
        }
    }
}
