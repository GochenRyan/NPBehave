using System;
using System.Linq;
using System.Reflection;
using static NPBehave.Action;

namespace NPSerialization
{
    public class DelegateData
    {
        [System.NonSerialized]
        public Action m_action;

        [System.NonSerialized]
        public Func<bool> m_singleFrameFunc;

        [System.NonSerialized]
        public Func<bool, Result> m_multiFrameFunc;

        [System.NonSerialized]
        public Func<Request, Result> m_multiFrameFunc2;

        public string ActionString
        {
            get
            {
                return m_action != null ? $"{m_action.Method.ReflectedType.FullName}|{m_action.Method.Name}" : null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    m_action = null;
                }
                else
                {
                    var parts = value.Split('|');
                    var typeName = parts.Length > 1 ? parts[0] : null;
                    var methodName = parts.Length > 1 ? parts[1] : parts[0];

                    var type = typeName != null ? Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName == typeName) : null;

                    if (type == null)
                    {
                        throw new ArgumentException($"Type {typeName} not found.");
                    }

                    var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                    if (method == null)
                    {
                        throw new ArgumentException($"Static method {methodName} not found in type {typeName}.");
                    }

                    // Ensure the method is static
                    if (!method.IsStatic)
                    {
                        throw new ArgumentException($"Method {methodName} in type {typeName} is not static.");
                    }

                    m_action = (Action)Delegate.CreateDelegate(typeof(Action), method);
                }
            }
        }

        public string SingleFrameFuncString
        {
            get
            {
                return SerializeFunc<bool>(m_singleFrameFunc);
            }
            set
            {
                m_singleFrameFunc = DeserializeFunc<bool>(value);
            }
        }

        public string MultiFrameFuncString
        {
            get
            {
                return SerializeFunc<bool, Result>(m_multiFrameFunc);
            }
            set
            {
                m_multiFrameFunc = DeserializeFunc<bool, Result>(value);
            }
        }

        public string MultiFrameFunc2String
        {
            get
            {
                return SerializeFunc<Request, Result>(m_multiFrameFunc2);
            }
            set
            {
                m_multiFrameFunc2 = DeserializeFunc<Request, Result>(value);
            }
        }

        private string SerializeFunc<T>(Func<T> func)
        {
            if (func == null)
                return null;

            var method = func.Method;
            var typeName = method.ReflectedType.FullName;
            var methodName = method.Name;

            return $"{typeName}|{methodName}";
        }

        private Func<T> DeserializeFunc<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var parts = value.Split('|');
            var typeName = parts[0];
            var methodName = parts[1];

            var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName == typeName);

            if (type == null)
            {
                throw new ArgumentException($"Type {typeName} not found.");
            }

            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found in type {typeName}.");
            }

            if (method.ReturnType != typeof(T))
            {
                throw new ArgumentException($"Method {methodName} in type {typeName} does not return {typeof(T)}.");
            }

            return (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), method);
        }

        private string SerializeFunc<T, TResult>(Func<T, TResult> func)
        {
            if (func == null)
                return null;

            var method = func.Method;
            var typeName = method.ReflectedType.FullName;
            var methodName = method.Name;

            return $"{typeName}|{methodName}";
        }

        private Func<T, TResult> DeserializeFunc<T, TResult>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            var parts = value.Split('|');
            var typeName = parts[0];
            var methodName = parts[1];

            var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName == typeName);

            if (type == null)
            {
                throw new ArgumentException($"Type {typeName} not found.");
            }

            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found in type {typeName}.");
            }

            if (method.ReturnType != typeof(TResult) || method.GetParameters().Length != 1 || method.GetParameters()[0].ParameterType != typeof(T))
            {
                throw new ArgumentException($"Method {methodName} in type {typeName} does not match Func<{typeof(T)}, {typeof(TResult)}> signature.");
            }

            return (Func<T, TResult>)Delegate.CreateDelegate(typeof(Func<T, TResult>), method);
        }
    }
}
