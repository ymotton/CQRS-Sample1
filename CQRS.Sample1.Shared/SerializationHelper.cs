using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace CQRS.Sample1.Shared
{
    public static class SerializationHelper
    {
        public static string Serialize(object instance)
        {
            var serializer = new DataContractSerializer(instance.GetType());

            using (var stringWriter = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                serializer.WriteObject(xmlWriter, instance);

                return stringWriter.ToString();
            }
        }
        public static object Deserialize(Type type, string serialized)
        {
            var serializer = new DataContractSerializer(type);

            using (var stringReader = new StringReader(serialized))
            using (var xmlReader = new XmlTextReader(stringReader))
            {
                return serializer.ReadObject(xmlReader);
            }
        }
    }
}
