using System.Threading;
using System.Threading.Tasks;
using BurgerJoint.Events;
using BurgerJoint.Rewards.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BurgerJoint.Rewards.Infrastructure
{
    public class EventListenerHostedService : BackgroundService
    {
        private readonly IOrderEventConsumer _eventConsumer;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventListenerHostedService(IOrderEventConsumer eventConsumer, IServiceScopeFactory scopeFactory)
        {
            _eventConsumer = eventConsumer;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => _eventConsumer.ConsumeAsync(@event => ForwardEventAsync(@event), stoppingToken);

        private async Task ForwardEventAsync(OrderEventBase @event)
        {
            using var scope = _scopeFactory.CreateScope();
            var eventHandler = scope.ServiceProvider.GetRequiredService<IOrderEventHandler>();
            await eventHandler.HandleAsync(@event);
        }
    }
}