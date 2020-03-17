using System;

namespace BurgerJoint.Events
{
    public abstract class OrderEventBase
    {
        public Guid Id { get; set; }
        
        public Guid OrderId { get; set; }

        public Guid DishId { get; set; }

        public string CustomerNumber { get; set; }
        
        public DateTime OccurredAt { get; set; }
    }
}