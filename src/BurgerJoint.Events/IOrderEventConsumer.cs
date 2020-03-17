using System;
using System.Threading;
using System.Threading.Tasks;

namespace BurgerJoint.Events
{
    public interface IOrderEventConsumer
    {
        Task ConsumeAsync(Func<OrderEventBase, Task> callback, CancellationToken ct);
    }
}