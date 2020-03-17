using System;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using BurgerJoint.Events;
using BurgerJoint.StoreFront.Data;
using BurgerJoint.StoreFront.Data.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderCancelled = BurgerJoint.StoreFront.Data.Events.OrderCancelled;
using OrderCreated = BurgerJoint.StoreFront.Data.Events.OrderCreated;
using OrderDelivered = BurgerJoint.StoreFront.Data.Events.OrderDelivered;

namespace BurgerJoint.StoreFront.Infrastructure
{
    public class OutboxMessagePublisher : IOutboxMessageListener
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Channel<bool> _channel;

        public OutboxMessagePublisher(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            // there's probably a smarter way to do this... but it'll do :)
            _channel = Channel.CreateBounded<bool>(
                new BoundedChannelOptions(1)
                {
                    FullMode = BoundedChannelFullMode.DropWrite
                });
        }

        public void OnNewMessages()
        {
            _channel.Writer.TryWrite(true);
        }

        public async Task StartAsync()
        {
            await foreach (var _ in _channel.Reader.ReadAllAsync())
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BurgerDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IOrderEventPublisher>();
                var messages = await db.OutboxMessages.OrderBy(o => o.OrderEventBase.OccurredAt).ToListAsync();
                foreach (var message in messages)
                {
                    await (message.OrderEventBase switch
                    {
                        OrderCreated created => publisher.PublishAsync(
                            new Events.OrderCreated
                            {
                                Id = message.Id,
                                DishId = created.DishId,
                                OrderId = created.OrderId,
                                CustomerNumber = created.CustomerNumber,
                                OccurredAt = created.OccurredAt
                            }),
                        OrderDelivered delivered => publisher.PublishAsync(
                            new Events.OrderDelivered
                            {
                                Id = message.Id,
                                DishId = delivered.DishId,
                                OrderId = delivered.OrderId,
                                CustomerNumber = delivered.CustomerNumber,
                                OccurredAt = delivered.OccurredAt
                            }),
                        OrderCancelled cancelled => publisher.PublishAsync(
                            new Events.OrderCancelled
                            {
                                Id = message.Id,
                                DishId = cancelled.DishId,
                                OrderId = cancelled.OrderId,
                                CustomerNumber = cancelled.CustomerNumber,
                                OccurredAt = cancelled.OccurredAt
                            }),
                        _ => throw new NotImplementedException()
                    });

                    if (messages.Any())
                    {
                        db.RemoveRange(messages);
                        await db.SaveChangesAsync();
                    }
                    
                    // NOTE: this is missing concurrency control, as the outbox might be read in parallel
                    // and messages published multiple times (this can always happen, but the least, the better)
                }
            }
        }

       public async Task StopAsync()
        {
            _channel.Writer.Complete();
            await _channel.Reader.Completion;
        }
    }
}