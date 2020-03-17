using System;

namespace BurgerJoint.Rewards.Data
{
    public class CustomerPurchase
    {
        public CustomerPurchase(string customerNumber, DateTime at)
        {
            CustomerNumber = customerNumber;
            At = at;
        }
        
        public long Id { get; private set; }
        
        public string CustomerNumber { get; private set; }

        public DateTime At { get; private set; }
    }
}