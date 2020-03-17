using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BurgerJoint.StoreFront.Infrastructure
{
    public class OutboxHostedService : IHostedService
    {
        private readonly OutboxMessagePublisher _publisher;
        private Task _processorTask;

        public OutboxHostedService(OutboxMessagePublisher publisher)
        {
            _publisher = publisher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _processorTask = _publisher.StartAsync();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => _publisher.StopAsync();
    }
}