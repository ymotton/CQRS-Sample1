using System;
using System.Runtime.Serialization;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Events
{
    [Serializable]
    [DataContract]
    public class ProductRenamed : Event
    {
        [DataMember]
        public string NewName { get; private set; }

        public ProductRenamed(Guid id, int version, string newName) : base(id, version)
        {
            NewName = newName;
        }
    }
}
