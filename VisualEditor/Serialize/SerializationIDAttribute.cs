using System;

namespace NPSerialization
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializationIDAttribute : Attribute
    {
        public SerializationIDAttribute()
        {
        }
    }
}