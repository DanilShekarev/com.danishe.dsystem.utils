using System;
using System.Globalization;
using System.Reflection;
using DSystemUtils.Dynamic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DSystemUtils.Editor
{
    public static class EditorUtils
    {
        public static bool DrawSerializedProp(Rect rect, Type type, ref string serializedData, ref Object objRef)
        {
            if (type == typeof(string))
            {
                serializedData = EditorGUI.TextField(rect, serializedData);
                return true;
            }
            if (type == typeof(int))
            {
                int.TryParse(serializedData, out var value);
                serializedData = EditorGUI.IntField(rect, value).ToString();
                return true;
            }
            if (type == typeof(float))
            {
                float.TryParse(serializedData, NumberStyles.Float, CultureInfo.InvariantCulture, out var value);
                serializedData = EditorGUI.FloatField(rect, value).ToString(CultureInfo.InvariantCulture);
                return true;
            }
            if (type == typeof(bool))
            {
                bool.TryParse(serializedData, out var value);
                serializedData = EditorGUI.Toggle(rect, value).ToString();
                return true;
            }
            if (type == typeof(LayerMask))
            {
                int.TryParse(serializedData, out var value);
                serializedData = LayerMaskDrawer.LayerMaskField(rect, (LayerMask)value).ToString();
                return true;
            }
            if (type == typeof(Object))
            {
                serializedData = null;
                objRef = EditorGUI.ObjectField(rect, objRef, type, true);
                return true;
            }
            if (type.IsEnum)
            {
                var names = Enum.GetNames(type);
                if (type.GetCustomAttribute<FlagsAttribute>() != null)
                {
                    int.TryParse(serializedData, out int mask);
                    mask = EditorGUI.MaskField(rect, mask, names);
                    serializedData = mask.ToString();
                }
                else
                {
                    var index = Array.IndexOf(names, serializedData);
                    if (index == -1)
                        index = 0;
                    index = EditorGUI.Popup(rect, index, names);
                    serializedData = names[index];
                }
                return true;
            }

            return false;
        }
        
        public static VisualElement DrawSerializedProp(Type type, string serializedData, Object objRef, 
            Action<string> serDataChanged, Action<Object> objectChanged)
        {
            if (type == typeof(string))
            {
                var ret = new TextField()
                {
                    value = serializedData,
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    serDataChanged?.Invoke(evt.newValue);
                });
                return ret;
            }
            if (type == typeof(int))
            {
                int.TryParse(serializedData, out var value);
                var ret = new IntegerField()
                {
                    value = value,
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    serDataChanged?.Invoke(evt.newValue.ToString(CultureInfo.InvariantCulture));
                });
                return ret;
            }
            if (type == typeof(float))
            {
                float.TryParse(serializedData, NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
                var ret = new FloatField()
                {
                    value = value,
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    serDataChanged?.Invoke(evt.newValue.ToString(CultureInfo.InvariantCulture));
                });
                return ret;
            }
            if (type == typeof(bool))
            {
                bool.TryParse(serializedData, out var value);
                var ret = new Toggle()
                {
                    value = value,
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    serDataChanged?.Invoke(evt.newValue.ToString(CultureInfo.InvariantCulture));
                });
                return ret;
            }
            if (type == typeof(LayerMask))
            {
                int.TryParse(serializedData, out var value);
                var ret = new LayerMaskField()
                {
                    value = value,
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    serDataChanged?.Invoke(evt.newValue.ToString(CultureInfo.InvariantCulture));
                });
                return ret;
            }
            if (typeof(Object).IsAssignableFrom(type))
            {
                var ret = new ObjectField()
                {
                    value = objRef,
                    objectType = type,
                    allowSceneObjects = true,
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    objectChanged?.Invoke(evt.newValue);
                });
                return ret;
            }
            if (typeof(Color).IsAssignableFrom(type))
            {
                var color = EventData<bool>.ParseColor(serializedData);
                var ret = new ColorField()
                {
                    value = color
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    serDataChanged?.Invoke(evt.newValue.ToString());
                });
                return ret;
            }
            if (type.IsEnum)
            {
                Enum.TryParse(type, serializedData, out var enumValue);

                var ret = new EnumField()
                {
                    value = (Enum)enumValue,
                };
                ret.RegisterValueChangedCallback(evt =>
                {
                    serDataChanged?.Invoke(evt.newValue.ToString());
                });
                // if (type.GetCustomAttribute<FlagsAttribute>() != null)
                // {
                //     int.TryParse(serializedData, out int mask);
                //     mask = EditorGUI.MaskField(rect, mask, names);
                //     serializedData = mask.ToString();
                // }
                // else
                // {
                //     var index = Array.IndexOf(names, serializedData);
                //     if (index == -1)
                //         index = 0;
                //     index = EditorGUI.Popup(rect, index, names);
                //     serializedData = names[index];
                // }
                return ret;
            }

            return null;
        }
        
        private const BindingFlags AllBindingFlags = (BindingFlags)(-1);

        /// <summary>
        /// Returns attributes of type <typeparamref name="TAttribute"/> on <paramref name="serializedProperty"/>.
        /// </summary>
        public static TAttribute[] GetAttributes<TAttribute>(this SerializedProperty serializedProperty, bool inherit)
            where TAttribute : Attribute
        {
            if (serializedProperty == null)
            {
                throw new ArgumentNullException(nameof(serializedProperty));
            }

            var targetObjectType = serializedProperty.serializedObject.targetObject.GetType();

            if (targetObjectType == null)
            {
                throw new ArgumentException($"Could not find the {nameof(targetObjectType)} of {nameof(serializedProperty)}");
            }
            
            while (targetObjectType != null)
            {
                foreach (var pathSegment in serializedProperty.propertyPath.Split('.'))
                {
                    var fieldInfo = targetObjectType.GetField(pathSegment, AllBindingFlags);
                    if (fieldInfo != null)
                    {
                        return (TAttribute[])fieldInfo.GetCustomAttributes<TAttribute>(inherit);
                    }

                    var propertyInfo = targetObjectType.GetProperty(pathSegment, AllBindingFlags);
                    if (propertyInfo != null)
                    {
                        return (TAttribute[])propertyInfo.GetCustomAttributes<TAttribute>(inherit);
                    }
                }

                targetObjectType = targetObjectType.BaseType;
            }

            throw new ArgumentException($"Could not find the field or property of {nameof(serializedProperty)}");
        }
    }
}