using System;
using Confluent.Kafka;

namespace BurgerJoint.Events.Kafka
{
    public class GuidSerializer: ISerializer<Guid>, IDeserializer<Guid>
    {
        private GuidSerializer()
        {
        }
        
        public static GuidSerializer Instance { get; } = new GuidSerializer();
        
        public byte[] Serialize(Guid data, SerializationContext context)
            => data.ToByteArray();

        public Guid Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            => new Guid(data);
    }
}