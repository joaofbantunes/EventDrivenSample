using System.Threading.Tasks;
using BurgerJoint.Events;
using BurgerJoint.Rewards.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BurgerJoint.Rewards.Domain
{
    // it should go without saying this is very very far from how something like this should be implemented 🙂

    public class RewardsProgramEventHandler : IOrderEventHandler
    {
        private readonly ILogger<RewardsProgramEventHandler> _logger;
        private readonly RewardsDbContext _db;

        public RewardsProgramEventHandler(ILogger<RewardsProgramEventHandler> logger, RewardsDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public Task HandleAsync(OrderEventBase @event)
            => @event switch
            {
                OrderDelivered delivered => HandleOrderDeliveredAsync(delivered),
                _ => Task.CompletedTask // this particular service only cares about successfully completed orders
            };

        private async Task HandleOrderDeliveredAsync(OrderDelivered delivered)
        {
            _db.CustomerPurchases.Add(new CustomerPurchase(delivered.CustomerNumber, delivered.OccurredAt));
            await _db.SaveChangesAsync();

            var purchaseCount = await _db
                .CustomerPurchases
                .CountAsync(p => p.CustomerNumber == delivered.CustomerNumber);

            if (purchaseCount % 3 == 0)
            {
                _logger.LogInformation(
                    "Customer has bought {purchaseCount} burgers. Maybe it's time for a reward!",
                    purchaseCount);
            }
        }
    }
}