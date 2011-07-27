using System;
using System.Runtime.Serialization;

namespace CQRS.Sample1.Shared
{
    [Serializable]
    [DataContract]
    public abstract class Message : IMessage
    {
        #region Properties
        
        [DataMember]
        public Guid Id { get; set; }
        
        [DataMember]
        public int Version { get; set; }

        #endregion

        #region Ctor

        protected Message(Guid id, int version)
        {
            Id = id;
            Version = version;
        }

        #endregion
    }
}