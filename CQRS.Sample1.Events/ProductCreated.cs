using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Events
{
    public class ProductCreated : Event
    {
        public string Name { get; private set; }

        public ProductCreated(Guid id, string name) : base(id)
        {
            Name = name;
        }
    }
}
