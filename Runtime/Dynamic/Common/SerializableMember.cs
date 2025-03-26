using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DSystemUtils.Dynamic
{
    [Serializable]
    public class SerializableMember
    {
        public enum MemberType { Method, Property }

        public string MemberName => _memberName;

        public Type DeclarationType => ResolveDeclaringType();

        [SerializeField] private MemberType _memberType;
        [SerializeField] private string _declaringTypeName;
        [SerializeField] private string _memberName;
        [SerializeField] private string[] _parameterTypeNames;

        public bool Exist
        {
            get
            {
                if (_memberType == MemberType.Property)
                    return GetProperty() != null;
                return GetMethod() != null;
            }
        }
        
        private Type _cachedDeclaringType;
        private MethodInfo _cachedMethod;
        private PropertyInfo _cachedProperty;

        public Type[] GetParameters()
        {
            if (_memberType == MemberType.Property)
                return new [] { GetProperty().PropertyType };
            return GetMethod().GetParameters().Select(s => s.ParameterType).ToArray();
        }

        public void Invoke(object target, object[] parameters)
        {
            if (_memberType == MemberType.Property)
            {
                SetPropertyValue(target, parameters[0]);
                return;
            }
            InvokeMethod(target, parameters);
        }

        public object InvokeGet(object target)
        {
            if (_memberType == MemberType.Property)
            {
                return GetPropertyValue(target);
            }
            return InvokeMethod(target, target);
        }

        public object InvokeMethod(object target, params object[] args)
        {
            var method = GetMethod();
            if (method == null)
            {
                Debug.LogError($"Method {_memberName} not found in type {target.GetType().Name}");
                return null;
            }
            if (method.GetParameters().Length == 0)
                return method.Invoke(target, null);
            return method.Invoke(target, args);
        }

        public void SetPropertyValue(object target, object value)
        {
            var property = GetProperty();
            property?.SetValue(target, value);
        }

        public object GetPropertyValue(object target)
        {
            var property = GetProperty();
            return property?.GetValue(target);
        }

        private MethodInfo GetMethod()
        {
            if (_cachedMethod == null) 
                _cachedMethod = GetMethod(_declaringTypeName, _memberName, _parameterTypeNames);
            
            return _cachedMethod;
        }

        public static MethodInfo GetMethod(string typeName, string methodName, string[] parameterTypeNames)
        {
            var type = Type.GetType(typeName);
            if (type == null) 
                return null;

            var method = type.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                null,
                ResolveParameterTypes(parameterTypeNames),
                null
            );

            return method;
        }

        private PropertyInfo GetProperty()
        {
            if (_cachedProperty != null) return _cachedProperty;

            var type = ResolveDeclaringType();
            if (type == null) return null;

            _cachedProperty = type.GetProperty(
                _memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
            );

            return _cachedProperty;
        }

        private Type ResolveDeclaringType()
        {
            if (_cachedDeclaringType != null) return _cachedDeclaringType;
            _cachedDeclaringType = Type.GetType(_declaringTypeName);
            return _cachedDeclaringType;
        }

        private static Type[] ResolveParameterTypes(string[] parameterTypeNames)
        {
            if (parameterTypeNames == null) 
                return Array.Empty<Type>();
            
            Type[] types = new Type[parameterTypeNames.Length];
            for (int i = 0; i < parameterTypeNames.Length; i++)
            {
                types[i] = Type.GetType(parameterTypeNames[i]);
            }
            return types;
        }

        private static string[] GetParameterTypeNames(ParameterInfo[] parameters)
        {
            string[] names = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                names[i] = parameters[i].ParameterType.AssemblyQualifiedName;
            }
            return names;
        }
    }
}