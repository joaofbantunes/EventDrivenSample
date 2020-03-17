namespace BurgerJoint.StoreFront.Data.Events
{
    public class OrderCancelled : OrderEventBase
    {
        public string Reason { get; set; }
    }
}