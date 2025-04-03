using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DSystemUtils.Dynamic
{
    [Serializable]
    public struct EventData<T>
    {
        public Object target;
        public SerializableMember targetMember;
        public bool[] useThis;
        public SerializableMember[] dataMembers;
        public string[] serializedValues;
        public Object[] serializedObjects;
        
        private Type[] _parameters;

        private object _invokeObject;

        public void Invoke(T val)
        {
            var type = val.GetType();
            var targetType = target.GetType();
            
            if (!targetMember.Exist)
            {
                Debug.LogError($"{targetType.Name} not have {targetMember.MemberName}!");
                return;
            }

            _invokeObject = targetMember.DeclarationType.IsAssignableFrom(target.GetType()) ? target : null;
            
            _parameters ??= targetMember.GetParameters();
            object[] parameters = new object[_parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (dataMembers.Length > i)
                {
                    var member = dataMembers[i];
                    if (useThis[i])
                        parameters[i] = val;
                    else if (member is not { Exist: true })
                    {
                        if (i == 0 && _parameters[i] == target.GetType())
                            parameters[i] = target;
                        else
                            parameters[i] = GetValue(i, _parameters[i]);   
                    }
                    else
                    {
                        if (member.Exist)
                            parameters[i] = member.InvokeGet(val);
                        else
                            Debug.LogError($"{type.Name} not have {member.MemberName}!");
                    }
                }
                else
                {
                    parameters[i] = GetValue(i, _parameters[i]);
                }
            }
            targetMember.Invoke(_invokeObject, parameters);
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