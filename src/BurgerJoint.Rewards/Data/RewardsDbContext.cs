using Microsoft.EntityFrameworkCore;

namespace BurgerJoint.Rewards.Data
{
    public class RewardsDbContext : DbContext
    {
        public RewardsDbContext(DbContextOptions<RewardsDbContext> options) : base(options)
        {
            
        }

        public DbSet<HandledEvent> HandledEvents { get; set; }

        public DbSet<CustomerPurchase> CustomerPurchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}