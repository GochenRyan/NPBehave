using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using static NPBehave.Action;

namespace NPSerialization
{
    public class DelegateData
    {
        [JsonIgnore]
        public Action m_action;

        [JsonIgnore]
        public Func<bool> m_singleFrameFunc;

        [JsonIgnore]
        public Func<bool, Result> m_multiFrameFunc;

        [JsonIgnore]
        public Func<Request, Result> m_multiFrameFunc2;

        public string ActionString
        {
            get
            {
                return GetSerializeString(m_action);
            }
            set
            {
                m_action = (Action)GetDelegate<Action>(value);
            }
        }

        public string SingleFrameFuncString
        {
            get
            {
                return GetSerializeString(m_singleFrameFunc);
            }
            set
            {
                m_singleFrameFunc = (Func<bool>)GetDelegate<Func<bool>>(value);
            }
        }

        public string MultiFrameFuncString
        {
            get
            {
                return GetSerializeString(m_multiFrameFunc);
            }
            set
            {
                m_multiFrameFunc = (Func<bool, Result>)GetDelegate<Func<bool, Result>>(value);
            }
        }

        public string MultiFrameFunc2String
        {
            get
            {
                return GetSerializeString(m_multiFrameFunc2);
            }
            set
            {

                m_multiFrameFunc2 = (Func<Request, Result>)GetDelegate<Func<Request, Result>>(value);
            }
        }

        private string GetSerializeString<T>(T delegateFunction) where T : System.Delegate
        {
            if (delegateFunction == null)
                return null;

            if (delegateFunction.Method.ReflectedType == null)
                return null;

            if (delegateFunction.Method.IsStatic)
            {
                return $"{delegateFunction.Method.ReflectedType.FullName}|{delegateFunction.Method.Name}";
            }
            else
            {
                object instance = delegateFunction.Target;
                if (instance == null)
                    return null;

                Type type = instance.GetType();
                var fields = type.GetFields();
                long IDObject = 0;
                foreach (var field in fields)
                {
                    if (Attribute.IsDefined(field, typeof(SerializationIDAttribute)))
                    {
                        IDObject = Convert.ToInt64(field.GetValue(instance));
                        break;
                    }
                }

                return $"{instance.GetType().FullName}|{IDObject}|{delegateFunction.Method.Name}";
            }
        }

        private Delegate GetDelegate<T>(string value) where T : System.Delegate
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                var parts = value.Split('|');
                string typeName = null;
                long ID = 0;
                string methodName = null;

                if (parts.Length == 2)
                {
                    typeName = parts[0];
                    methodName = parts[1];
                }
                else if (parts.Length == 3)
                {
                    typeName = parts[0];
                    ID = Convert.ToInt64(parts[1]);
                    methodName = parts[2];
                }

                var type = typeName != null ? Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName == typeName) : null;

                if (type == null)
                {
                    throw new ArgumentException($"Type {typeName} not found.");
                }

                if (methodName == null)
                {
                    return null;
                }

                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                if (method == null)
                {
                    throw new ArgumentException($"Static method {methodName} not found in type {typeName}.");
                }

                if (!method.IsStatic)
                {
                    var fields = type.GetFields();
                    object instance;
                    if (ID == 0 || !InstanceContext.Instance.TryGetReference(type, ID, out instance))
                    {
                        return null;
                    }

                    return (T)Delegate.CreateDelegate(typeof(T), instance, method);
                }

                return (T)Delegate.CreateDelegate(typeof(T), method);
            }
        }
    }
}
