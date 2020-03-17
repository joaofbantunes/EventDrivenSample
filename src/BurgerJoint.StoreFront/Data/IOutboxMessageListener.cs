namespace BurgerJoint.StoreFront.Data
{
    public interface IOutboxMessageListener
    {
        void OnNewMessages();
    }
}