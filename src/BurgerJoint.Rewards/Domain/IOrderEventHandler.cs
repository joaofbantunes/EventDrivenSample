using System.Threading.Tasks;
using BurgerJoint.Events;

namespace BurgerJoint.Rewards.Domain
{
    public interface IOrderEventHandler
    {
        Task HandleAsync(OrderEventBase @event);
    }
}