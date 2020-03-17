using System;

namespace BurgerJoint.StoreFront.Data.Events
{
    public abstract class OrderEventBase
    { 
        public Guid OrderId { get; set; }

        public Guid DishId { get; set; }

        public string CustomerNumber { get; set; }
        
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }
}