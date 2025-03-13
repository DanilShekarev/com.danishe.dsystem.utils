using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DSystemUtils.Dynamic
{
    [Serializable]
    public class ValueProvider<T>
    {
        [SerializeField] private Object target;
        [SerializeField] private string methodName;
        [SerializeField] private T defaultValue;
        
        private Type _type;
        private MethodInfo _methodInfo;

        public T GetValue()
        {
            if (string.IsNullOrEmpty(methodName)) 
                return defaultValue;
            
            _type ??= target.GetType();
            
            if (!string.IsNullOrEmpty(methodName) && _methodInfo == null)
                _methodInfo = _type.GetMethod(methodName, 
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (_methodInfo == null) 
                return defaultValue;
            
            return (T)_methodInfo.Invoke(target, null);
        }
    }
}