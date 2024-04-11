using NPSerialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

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
            else if (type == typeof(float))
            {
                element = new FloatField(label)
                {
                    value = (float)fieldInfo.GetValue(obj)
                };
            }
            else if (type == typeof(string))
            {
                element = new TextField(label)
                {
                    value = (string)fieldInfo.GetValue(obj)
                };
            }
            else if (type.IsEnum) 
            {
                element = new EnumField(label, (Enum)fieldInfo.GetValue(obj));
            }
            else if (type == typeof(DelegateData))
            {
                element = new VisualElement();
                element.style.borderBottomWidth = 1;
                element.style.borderBottomColor = Color.yellow;
                element.style.borderTopWidth = 1;
                element.style.borderTopColor = Color.yellow;

                element.style.flexDirection = FlexDirection.Column;
                var methodContanier = new VisualElement();
                methodContanier.style.flexDirection = FlexDirection.Row;

                DelegateData delegateData = (DelegateData)fieldInfo.GetValue(obj);
                string funcName = delegateData.GetMethodName();
                var funcElement = new TextField(label)
                {
                    value = funcName
                };
                var objectField = new ObjectField("New Method");
                var dropdownField = new DropdownField();
                objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(selectedObject => {
                    List<string> funcList = new();
                    var methods = selectedObject.GetType().GetMethods();
                    foreach (var method in methods)
                    {
                        funcList.Add(method.Name);
                    }
                    dropdownField.choices = funcList;
                });
                
                methodContanier.Add(objectField);
                methodContanier.Add(dropdownField);

                element.Add(funcElement);
                element.Add(methodContanier);
            }


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