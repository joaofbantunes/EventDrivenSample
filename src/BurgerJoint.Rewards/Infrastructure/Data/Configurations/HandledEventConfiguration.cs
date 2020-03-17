using BurgerJoint.Rewards.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BurgerJoint.Rewards.Infrastructure.Data.Configurations
{
    public class HandledEventConfiguration : IEntityTypeConfiguration<HandledEvent>
    {
        public void Configure(EntityTypeBuilder<HandledEvent> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}