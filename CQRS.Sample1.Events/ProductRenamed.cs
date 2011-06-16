using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Events
{
    public class ProductRenamed : Event
    {
        public string NewName { get; private set; }

        public ProductRenamed(Guid id, string newName) : base(id)
        {
            NewName = newName;
        }
    }
}
