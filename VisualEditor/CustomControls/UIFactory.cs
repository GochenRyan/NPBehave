using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace NPVisualEditor
{
    public static class UIFactory
    {
        public static List<VisualElement> CreateElements(object obj)
        {
            List<VisualElement> elements = new();
            foreach (FieldInfo info in obj.GetType().GetFields())
            {
                bool read = IsRead(obj, info);
                EventCallback<FocusOutEvent> eventCallback = GetEventCallBack(obj, info);
                VisualElement element = CreateElement(obj, info, read, eventCallback);
                if (element != null)
                {
                    elements.Add(element);
                }
            }

            return elements;
        }

        public static VisualElement CreateElement(object obj, FieldInfo fieldInfo, bool read, EventCallback<FocusOutEvent> callback)
        {
            VisualElement element = null;

            Type type = fieldInfo.FieldType;
            string label = GetLabel(fieldInfo.Name);
            if (type == typeof(long))
            {
                element = new LongField(label)
                {
                    value = (long)fieldInfo.GetValue(obj)
                };
            }
            else if (type == typeof(string))
            {
                element = new TextField(label)
                {
                    value = (string)fieldInfo.GetValue(obj)
                };
            }

            // TODO: Enum

            if (element != null)
            {
                if (read)
                {
                    element.SetEnabled(false);
                }
                else
                {
                    if (callback != null)
                    {
                        element.RegisterCallback(callback);
                    }
                }
            }

            return element;
        }

        private static string GetLabel(string name)
        {
            if (name.StartsWith("m_"))
                return name[2..];
            return name;
        }

        private static bool IsRead(object obj, FieldInfo fieldInfo)
        {
            return false;
        }

        private static EventCallback<FocusOutEvent> GetEventCallBack(object obj, FieldInfo fieldInfo)
        {
            return null;
        }
    }
}