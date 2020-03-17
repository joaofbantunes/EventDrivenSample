using System.Threading.Tasks;

namespace BurgerJoint.Events
{
    public interface IOrderEventPublisher
    {
        Task PublishAsync(OrderEventBase orderEventBase);
    }
}