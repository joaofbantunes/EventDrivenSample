using System;

namespace BurgerJoint.Rewards.Data
{
    public class HandledEvent
    {
        public Guid Id { get; private set; }

        public DateTime ReceivedAt { get; private set; }

        public static HandledEvent Create(Guid id)
            => new HandledEvent {Id = id, ReceivedAt = DateTime.UtcNow};
    }
}