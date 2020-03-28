using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BurgerJoint.StoreFront.Infrastructure
{
    public class OutboxHostedService : BackgroundService
    {
        private readonly OutboxMessagePublisher _publisher;

        public OutboxHostedService(OutboxMessagePublisher publisher)
        {
            _publisher = publisher;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => _publisher.RunAsync(stoppingToken);
    }
}