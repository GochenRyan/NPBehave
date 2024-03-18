using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System;

namespace NPSerialization
{
    /// <summary>
    /// See https://stackoverflow.com/questions/8030538/how-to-implement-custom-jsonconverter-in-json-net
    /// </summary>
    public class NodeDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return System.Attribute.GetCustomAttributes(objectType).Any(v => v is KnownTypeAttribute);
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // load the object 
            JObject jObject = JObject.Load(reader);

            // take custom attributes on the type
            Attribute[] attrs = Attribute.GetCustomAttributes(objectType);

            // take the names of elements from json data
            HashSet<string> jObjectKeys = GetKeys(jObject);

            JToken valueToken = jObject.GetValue("TYPE_NAME_FOR_SERIALIZATION");

            if (valueToken != null && valueToken.Type == JTokenType.String)
            {
                string typeName = valueToken.Value<string>();
                Type nodeDataType = null;

                // trying to find the right type
                foreach (var attr in attrs.OfType<KnownTypeAttribute>())
                {
                    Type knownType = attr.Type;
                    if (!objectType.IsAssignableFrom(knownType))
                        continue;

                    if (knownType.FullName == typeName)
                    {
                        nodeDataType = knownType;
                        break;
                    }
                }
                if (nodeDataType != null)
                {
                    object target = Activator.CreateInstance(nodeDataType);
                    using (JsonReader jObjectReader = CopyReaderForObject(reader, jObject))
                    {
                        serializer.Populate(jObjectReader, target);
                    }
                    return target;
                }
            }
            throw new SerializationException($"Could not serialize to KnownTypes and assign to base class {objectType} reference");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private HashSet<string> GetKeys(JObject obj)
        {
            return new HashSet<string>(((IEnumerable<KeyValuePair<string, JToken>>)obj).Select(k => k.Key));
        }

        public static JsonReader CopyReaderForObject(JsonReader reader, JObject jObject)
        {
            JsonReader jObjectReader = jObject.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateFormatString = reader.DateFormatString;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;
            jObjectReader.MaxDepth = reader.MaxDepth;
            jObjectReader.SupportMultipleContent = reader.SupportMultipleContent;
            return jObjectReader;
        }
    }
}