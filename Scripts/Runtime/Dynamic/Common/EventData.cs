using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DSystemUtils.Dynamic
{
    [Serializable]
    public struct EventData
    {
        public Object target;
        public string methodName;
        public string[] methodsParamsTypes;
        public string[] propertyNames;
        public string[] serializedValues;
        public Object[] serializedObjects;
        
        private MethodInfo _method;
        private ParameterInfo[] _parameters;

        public void Invoke(object val)
        {
            var type = val.GetType();
            var targetType = target.GetType();
            if (_method == null)
            {
                if (GetParameters(out var parametersTypes))
                    _method = targetType.GetMethod(GetMethodName(), parametersTypes);
                else
                    _method = targetType.GetMethod(GetMethodName());
            }
            
            if (_method == null)
            {
                Debug.LogError($"{targetType.Name} not have method {methodName}!");
                return;
            }
            _parameters ??= _method.GetParameters();
            object[] parameters = new object[_parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (propertyNames.Length > i)
                {
                    string propertyName = propertyNames[i];
                    if (string.IsNullOrEmpty(propertyName) || propertyName == "None")
                        parameters[i] = GetValue(i, _parameters[i].ParameterType);
                    else if (propertyName == "this")
                        parameters[i] = val;
                    else
                    {
                        var propertyInfo = type.GetProperty(propertyName);
                        if (propertyInfo == null)
                            Debug.LogError($"{type.Name} not have property {propertyName}!");
                        else
                            parameters[i] = propertyInfo.GetValue(val);
                    }
                }
                else
                {
                    parameters[i] = GetValue(i, _parameters[i].ParameterType);
                }
            }
            _method?.Invoke(target, parameters);
        }

        private string GetMethodName()
        {
            var ret = methodName.Substring(0, methodName.IndexOf('(')-1);
            return ret;
        }

        private bool GetParameters(out Type[] parametersTypes)
        {
            if (methodsParamsTypes == null || methodsParamsTypes.Length == 0)
            {
                parametersTypes = null;
                return false;
            }
            
            Type[] types = new Type[methodsParamsTypes.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = Type.GetType(methodsParamsTypes[i]);
            }
            
            parametersTypes = types;
            return true;
        }

        public object GetValue(int i, Type type)
        {
            if (type.IsSubclassOf(typeof(Object)))
            {
                return serializedObjects[i];
            }
            if (type == typeof(int))
            {
                int.TryParse(serializedValues[i], out var ret);
                return ret;
            }
            if (type == typeof(float))
            {
                float.TryParse(serializedValues[i], out var ret);
                return ret;
            }
            if (type == typeof(bool))
            {
                bool.TryParse(serializedValues[i], out var ret);
                return ret;
            }
            if (type.IsEnum)
            {
                var names = type.GetEnumNames();
                if (type.HasAttribute<FlagsAttribute>())
                {
                    int.TryParse(serializedValues[i], out var mask);
                    return Enum.ToObject(type, mask);
                }
                var index = Array.IndexOf(names, serializedValues[i]);
                var enums = type.GetEnumValues();
                return enums.GetValue(index);
            }
            if (type == typeof(string))
            {
                return serializedValues[i];
            }
            return default;
        }
    }
}