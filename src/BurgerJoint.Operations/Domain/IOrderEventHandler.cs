using System.Threading.Tasks;
using BurgerJoint.Events;

namespace BurgerJoint.Operations.Domain
{
    public interface IOrderEventHandler
    {
        Task HandleAsync(OrderEventBase @event);
    }
}