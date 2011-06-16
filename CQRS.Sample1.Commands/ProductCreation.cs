using System;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Commands
{
    public class ProductCreation : Command
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            private set
            {
                // Validation can be handled in Commands, so we are sure the command is valid before we send it.
                // This minimizes traffic in case it is something we could have caught clientside
                if (string.IsNullOrWhiteSpace(value)) new ArgumentException("The product name should be filled in.");
                _name = value;
            }
        }

        public ProductCreation(Guid id, string name) : base(id)
        {
            Name = name;
        }
    }
}
