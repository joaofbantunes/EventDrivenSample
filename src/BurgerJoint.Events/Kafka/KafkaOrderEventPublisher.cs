using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace BurgerJoint.Events.Kafka
{
    public class KafkaOrderEventPublisher : IOrderEventPublisher
    {
        private readonly ILogger<KafkaOrderEventPublisher> _logger;
        private readonly IProducer<Guid, OrderEventBase> _producer;

        public KafkaOrderEventPublisher(ILogger<KafkaOrderEventPublisher> logger)
        {
            _logger = logger;
            
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };
            
            _producer = new ProducerBuilder<Guid, OrderEventBase>(config)
                .SetKeySerializer(GuidSerializer.Instance)
                .SetValueSerializer(JsonEventSerializer<OrderEventBase>.Instance)
                .Build();
        }

        public async Task PublishAsync(OrderEventBase orderEventBase)
        {
            _logger.LogInformation(
                "Publishing event {eventId}, of type {eventType}!",
                orderEventBase.Id,
                orderEventBase.GetType().Name);

            await _producer.ProduceAsync("orders", new Message<Guid, OrderEventBase>
            {
                // use order id as key so Kafka maintains order between events for the same entity
                Key = orderEventBase.OrderId,
                Value = orderEventBase
            });
        }
    }
}