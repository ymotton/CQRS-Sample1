using System;
using System.Runtime.Serialization;
using CQRS.Sample1.Shared;

namespace CQRS.Sample1.Events
{
    [Serializable]
    [DataContract]
    public class ProductCreated : Event
    {
        [DataMember]
        public string Name { get; private set; }

        public ProductCreated(Guid id, int version, string name) : base(id, version)
        {
            Name = name;
        }
    }
}
