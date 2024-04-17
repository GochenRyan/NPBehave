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

        [JsonIgnore]
        public string m_actionName = string.Empty;
        [JsonIgnore]
        public string m_singleFrameFuncName = string.Empty;
        [JsonIgnore]
        public string m_multiFrameFuncName = string.Empty;
        [JsonIgnore]
        public string m_multiFrameFunc2Name = string.Empty;

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
                long IDObject = 0;
                object[] attributes = type.GetCustomAttributes(typeof(SerializationIDAttribute), false);

                foreach (var attribute in attributes)
                {
                    if (attribute is SerializationIDAttribute serializationID)
                    {
                        IDObject = serializationID.ID;
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
    
        public string GetMethodName()
        {
            string funcName = string.Empty;
            if (m_action != null)
            {
                funcName = GetSerializeString(m_action);
            }
            else if (m_singleFrameFunc != null)
            {
                funcName = GetSerializeString(m_singleFrameFunc);
            }
            else if (m_multiFrameFunc != null)
            {
                funcName = GetSerializeString(m_multiFrameFunc);
            }
            else if (m_multiFrameFunc2 != null)
            {
                funcName = GetSerializeString(m_multiFrameFunc2);
            }

            return funcName;
        }
    
        public bool ResetMethod(string methodString)
        {
            string[] parts = methodString.Split('|');
            if (parts.Length != 3 && parts.Length != 2)
                return false;

            string typeName;
            string methodName;
            MethodInfo methodInfo = null;
            if (parts.Length == 2)
            {
                typeName = parts[0];
                methodName = parts[1];
            }
            else
            {
                typeName = parts[0];
                methodName = parts[2];
            }

            Type type = Type.GetType(typeName);
            if (type == null)
                return false;

            methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (methodInfo == null)
                return false;

            Type[] parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            if (methodInfo.ReturnType == typeof(void))
            {
                if (parameterTypes.Length == 0)
                {
                    Clear();
                    ActionString = methodString;
                    return true;
                }
                else if (parameterTypes.Length == 1 && parameterTypes[0] == typeof(bool))
                {
                    Clear();
                    SingleFrameFuncString = methodString;
                    return true;
                }
            }
            else if (methodInfo.ReturnType == typeof(Result))
            {
                if (parameterTypes.Length == 1 && parameterTypes[0] == typeof(bool))
                {
                    Clear();
                    MultiFrameFuncString = methodString;
                    return true;
                }
                else if (parameterTypes.Length == 1 && parameterTypes[0] == typeof(Request))
                {
                    Clear();
                    MultiFrameFunc2String = methodString;
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            m_action = null;
            m_singleFrameFunc = null;
            m_multiFrameFunc = null;
            m_multiFrameFunc2 = null;
        }
    }
}
