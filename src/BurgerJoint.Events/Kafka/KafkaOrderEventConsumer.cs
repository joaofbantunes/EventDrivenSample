using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace BurgerJoint.Events.Kafka
{
    public class KafkaOrderEventConsumer : IOrderEventConsumer
    {
        private readonly ILogger<KafkaOrderEventConsumer> _logger;
        private readonly IConsumer<Guid, OrderEventBase> _consumer;

        public KafkaOrderEventConsumer(
            ILogger<KafkaOrderEventConsumer> logger,
            KafkaOrderEventConsumerSettings settings)
        {
            _logger = logger;

            var conf = new ConsumerConfig
            {
                GroupId = settings.ConsumerGroup,
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Guid, OrderEventBase>(conf)
                .SetKeyDeserializer(GuidSerializer.Instance)
                .SetValueDeserializer(JsonEventSerializer<OrderEventBase>.Instance)
                .Build();
        }

        // using Task.Run just because polling blocks
        // after the await, the initial task will be let go,
        // so no great advantage in doing Task.Factory.StartNew(x, LongRunning)
        // TODO: find out if there is a better approach for this
        public Task ConsumeAsync(Func<OrderEventBase, Task> callback, CancellationToken ct)
            => Task.Run(async () =>
            {
                _logger.LogInformation("Subscribing");
                _consumer.Subscribe("orders");

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Waiting for message...");

                        var message = _consumer.Consume(ct);

                        _logger.LogInformation(
                            "Received event {eventId}, of type {eventType}!",
                            message.Value.Id,
                            message.Value.GetType().Name);

                        await callback(message.Value);

                        _consumer.Commit(); // note: committing every time can have a negative impact on performance
                    }
                    catch (Exception ex)
                    {
                        // TODO: implement error handling/retry logic
                        // like this, the failed message will eventually be "marked as processed"
                        // (commit to a newer offset) even though it failed
                        _logger.LogError(ex, "Error occurred when consuming event!");
                    }
                }
            });
    }
}