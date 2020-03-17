using System;
using System.Threading.Tasks;
using BurgerJoint.Events;
using BurgerJoint.Rewards.Data;
using BurgerJoint.Rewards.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace BurgerJoint.Rewards.Infrastructure
{
    public class OrderEventHandlerIdempotenceDecorator : IOrderEventHandler
    {
        private readonly IOrderEventHandler _next;
        private readonly RewardsDbContext _db;
        private readonly ILogger<OrderEventHandlerIdempotenceDecorator> _logger;

        public OrderEventHandlerIdempotenceDecorator(
            IOrderEventHandler next, 
            RewardsDbContext db,
            ILogger<OrderEventHandlerIdempotenceDecorator> logger)
        {
            _next = next;
            _db = db;
            _logger = logger;
        }

        public async Task HandleAsync(OrderEventBase @event)
        {
            try
            {
                await _db.Database.BeginTransactionAsync();

                if (!await TryAddHandledEvent(@event.Id))
                {
                    // if event already handled, just rollback return
                    _logger.LogInformation("Event {eventId} already handled, ignoring.", @event.Id);
                    _db.Database.RollbackTransaction();
                    return;
                }

                await _next.HandleAsync(@event);

                _db.Database.CommitTransaction();
            }
            catch (Exception)
            {
                _db.Database.RollbackTransaction();
                throw;
            }

            async Task<bool> TryAddHandledEvent(Guid eventId)
            {
                const string DuplicateKeySqlState = "23505";
                try
                {
                    _db.HandledEvents.Add(HandledEvent.Create(@event.Id));
                    await _db.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException ex)
                    when (ex.InnerException is PostgresException pex
                          && pex.SqlState == DuplicateKeySqlState)
                {
                    return false;
                }
            }
        }
    }
}