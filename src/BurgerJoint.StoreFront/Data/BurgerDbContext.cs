using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BurgerJoint.StoreFront.Data.Events;
using Microsoft.EntityFrameworkCore;

namespace BurgerJoint.StoreFront.Data
{
    public class BurgerDbContext : DbContext
    {
        private readonly IOutboxMessageListener _outboxMessageListener;

        public BurgerDbContext(DbContextOptions<BurgerDbContext> options, IOutboxMessageListener outboxMessageListener) : base(options)
        {
            _outboxMessageListener = outboxMessageListener;
        }
        
        public DbSet<Dish> Dishes { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // detect and persist entity events
            PersistOrderDeliveredEvents();
            
            // persist events explicitly created by the entities
            PersistEntityEvents();

            var newMessagesToDeliverExist = ChangeTracker.Entries<OutboxMessage>().Any();
            
            var result = await base.SaveChangesAsync(cancellationToken);

            if (newMessagesToDeliverExist)
            {
                _outboxMessageListener.OnNewMessages();
            }
            
            return result;
            
            void PersistOrderDeliveredEvents()
            {
                var deliveredOrders = ChangeTracker
                    .Entries()
                    .Where(e => e.State == EntityState.Modified
                                && e.Entity is Order order
                                && (Status) e.OriginalValues[nameof(order.Status)] == Status.Pending
                                && (Status) e.CurrentValues[nameof(order.Status)] == Status.Delivered)
                    .Select(e =>
                    {
                        var order = (Order) e.Entity;
                        return OutboxMessage.Create(
                            new OrderDelivered {OrderId = order.Id, DishId = order.Dish.Id, CustomerNumber = order.CustomerNumber});
                    });

                OutboxMessages.AddRange(deliveredOrders);
            }

            void PersistEntityEvents()
            {
                var messages = ChangeTracker
                    .Entries()
                    .Where(e => e.Entity is EntityBase entity)
                    .SelectMany(e =>
                    {
                        var entity = (EntityBase) e.Entity;
                        return entity.Events.Select(evt => OutboxMessage.Create(evt));
                    });

                OutboxMessages.AddRange(messages);
            }
        }
    }
}