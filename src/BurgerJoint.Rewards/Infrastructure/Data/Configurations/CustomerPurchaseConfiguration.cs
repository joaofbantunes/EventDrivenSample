using BurgerJoint.Rewards.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BurgerJoint.Rewards.Infrastructure.Data.Configurations
{
    public class CustomerPurchaseConfiguration : IEntityTypeConfiguration<CustomerPurchase>
    {
        public void Configure(EntityTypeBuilder<CustomerPurchase> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseIdentityAlwaysColumn();
        }
    }
}