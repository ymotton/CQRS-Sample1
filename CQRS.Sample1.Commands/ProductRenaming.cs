using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Commands
{
    public class ProductRenaming : Command
    {
        public string NewName { get; private set; }

        public ProductRenaming(Guid id, string newName) : base(id)
        {
            NewName = newName;
        }
    }
}
