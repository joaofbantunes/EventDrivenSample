using BurgerJoint.StoreFront.Data;
using BurgerJoint.StoreFront.Data.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace BurgerJoint.StoreFront.Infrastructure.Data.Configurations
{
    public class OutboxMessageConfiguration: IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            
            builder.HasKey(e => e.Id);
            builder
                .Property(e => e.OrderEventBase)
                .HasColumnType("jsonb")
                // Npgsql supports JSON out of the box, but doesn't handle hierarchies, so just using Newtonsoft to avoid more work 
                .HasConversion(
                    e => JsonConvert.SerializeObject(e, settings),
                    e => JsonConvert.DeserializeObject<OrderEventBase>(e, settings));
        }
    }
}