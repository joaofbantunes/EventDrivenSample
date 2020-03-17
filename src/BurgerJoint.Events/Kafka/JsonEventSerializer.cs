using System;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace BurgerJoint.Events.Kafka
{
    public class JsonEventSerializer<T> : ISerializer<T>, IDeserializer<T>  where T : class
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        private JsonEventSerializer()
        {
        }

        public static JsonEventSerializer<T> Instance { get; } = new JsonEventSerializer<T>();

        public byte[] Serialize(T data, SerializationContext context)
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Settings));
        
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            => isNull
                ? null
                : JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data), Settings);
    }

}