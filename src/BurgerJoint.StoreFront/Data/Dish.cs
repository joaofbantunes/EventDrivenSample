using System;

namespace BurgerJoint.StoreFront.Data
{
    public class Dish
    {
        public Dish(Guid id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }
    }
}