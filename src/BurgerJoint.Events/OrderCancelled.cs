namespace BurgerJoint.Events
{
    public class OrderCancelled : OrderEventBase
    {
        public string Reason { get; set; }
    }
}