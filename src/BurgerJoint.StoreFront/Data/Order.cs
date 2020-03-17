using System;
using BurgerJoint.StoreFront.Data.Events;

namespace BurgerJoint.StoreFront.Data
{
    public class Order : EntityBase
    {
        public Guid Id { get; private set; }

        public Dish Dish { get; private set; }

        public Status Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public string CustomerNumber { get; private set; }

        // outbox completely handled by DbContext
        public void Deliver()
        {
            Status = Status.Delivered;
        }

        // outbox handled in tandem by Order (using EntityBase.Events) and DbContext
        public void Cancel(string reason)
        {
            Status = Status.Cancelled;
            AddEvent(new OrderCancelled
            {
                OrderId = Id,
                DishId = Dish.Id,
                CustomerNumber = CustomerNumber,
                Reason = reason
            });
        }

        // without outbox worries (not good)
        public static Order Create(Dish dish, string customerNumber)
            => new Order
            {
                Id = Guid.NewGuid(),
                Status = Status.Pending,
                Dish = dish,
                CreatedAt = DateTime.UtcNow,
                CustomerNumber = customerNumber
            };
    }
}