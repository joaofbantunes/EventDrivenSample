using System.Collections.Generic;
using BurgerJoint.StoreFront.Data.Events;

namespace BurgerJoint.StoreFront.Data
{
    public abstract class EntityBase
    {
        private List<OrderEventBase> _events = new List<OrderEventBase>();

        public IReadOnlyCollection<OrderEventBase> Events => _events.AsReadOnly(); 

        protected void AddEvent(OrderEventBase orderEventBase)
        {
            _events.Add(orderEventBase);
        }
    }
}