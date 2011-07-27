using System;
using System.Runtime.Serialization;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Commands
{
    [DataContract]
    [Serializable]
    public class ProductRenaming : Command
    {
        [DataMember]
        public string NewName
        {
            get { return _newName; }
            private set
            {
                // Validation can be handled in Commands, so we are sure the command is valid before we send it.
                // This minimizes traffic in case it is something we could have caught clientside
                if (string.IsNullOrWhiteSpace(value)) new ArgumentException("The product name should be filled in.");
                _newName = value;
            }
        }
        private string _newName;

        public ProductRenaming(Guid id, string newName, int version) : base(id, version)
        {
            NewName = newName;
        }
    }
}
