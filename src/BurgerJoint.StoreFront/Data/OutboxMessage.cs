using System;
using BurgerJoint.StoreFront.Data.Events;

namespace BurgerJoint.StoreFront.Data
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public OrderEventBase OrderEventBase { get; private set; }
        
        public static OutboxMessage Create(OrderEventBase orderEventBase)
            => new OutboxMessage{Id = Guid.NewGuid(), OrderEventBase = orderEventBase};
    }
}