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
    public class KnownTypeConverter : JsonConverter
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

            Type mostSuitableType = null;
            int countOfMaxMatchingProperties = -1;

            // take the names of elements from json data
            HashSet<string> jObjectKeys = GetKeys(jObject);

            // take the properties of the parent class (in our case, from the Document class, which is specified in DocumentsRequestIdResponse)
            HashSet<string> objectTypeProps = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => p.Name)
                .ToHashSet();

            // trying to find the right "KnownType"
            foreach (var attr in attrs.OfType<KnownTypeAttribute>())
            {
                Type knownType = attr.Type;
                if (!objectType.IsAssignableFrom(knownType))
                    continue;

                // select properties of the inheritor, except properties from the parent class and properties with "ignore" attributes (in our case JsonIgnoreAttribute and XmlIgnoreAttribute)
                var notIgnoreProps = knownType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => !objectTypeProps.Contains(p.Name)
                                && p.CustomAttributes.All(a => a.AttributeType != typeof(JsonIgnoreAttribute) && a.AttributeType != typeof(System.Xml.Serialization.XmlIgnoreAttribute)));

                //  get serializable property names
                var jsonNameFields = notIgnoreProps.Select(prop =>
                {
                    string jsonFieldName = null;
                    CustomAttributeData jsonPropertyAttribute = prop.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(JsonPropertyAttribute));
                    if (jsonPropertyAttribute != null)
                    {
                        // take the name of the json element from the attribute constructor
                        CustomAttributeTypedArgument argument = jsonPropertyAttribute.ConstructorArguments.FirstOrDefault();
                        if (argument != null && argument.ArgumentType == typeof(string) && !string.IsNullOrEmpty((string)argument.Value))
                            jsonFieldName = (string)argument.Value;
                    }
                    // otherwise, take the name of the property
                    if (string.IsNullOrEmpty(jsonFieldName))
                    {
                        jsonFieldName = prop.Name;
                    }

                    return jsonFieldName;
                });


                HashSet<string> jKnownTypeKeys = new HashSet<string>(jsonNameFields);

                // by intersecting the sets of names we determine the most suitable inheritor
                int count = jObjectKeys.Intersect(jKnownTypeKeys).Count();

                if (count == jKnownTypeKeys.Count)
                {
                    mostSuitableType = knownType;
                    break;
                }

                if (count > countOfMaxMatchingProperties)
                {
                    countOfMaxMatchingProperties = count;
                    mostSuitableType = knownType;
                }
            }

            if (mostSuitableType != null)
            {
                object target = Activator.CreateInstance(mostSuitableType);
                using (JsonReader jObjectReader = CopyReaderForObject(reader, jObject))
                {
                    serializer.Populate(jObjectReader, target);
                }
                return target;
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