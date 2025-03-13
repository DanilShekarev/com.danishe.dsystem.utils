using System;
using System.Collections.Generic;
using System.Reflection;
using DSystemUtils.Dynamic;
using UnityEngine;

namespace DSystemUtils.Reflection
{
    public static class ReflectionExtension
    {
        public static Type GetType(string typeName) {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assembly.GetTypes()) {
                    if (type.Name == typeName) {
                        return type;
                    }
                }
            }
            return null;
        }

        public static bool TryDisposeSubscribe(this object obj, IEventDisposable.PreDisposeDelegate preDisposeDelegate)
        {
            if (obj is not IEventDisposable disposable)
            {
                if (obj is GameObject gameObject)
                {
                    if (!gameObject.TryGetComponent(out DisposeListener disposableListener))
                        disposableListener = gameObject.AddComponent<DisposeListener>();
                    disposableListener.Dispose += preDisposeDelegate;
                    return true;
                }
                return false;
            }
            disposable.PreDispose += preDisposeDelegate;
            return true;
        }
        
        public static bool TryDisposeUnsubscribe(this object obj, IEventDisposable.PreDisposeDelegate preDisposeDelegate)
        {
            if (obj is not IEventDisposable disposable)
            {
                if (obj is GameObject gameObject)
                {
                    if (!gameObject.TryGetComponent(out DisposeListener disposableListener)) return false;
                    disposableListener.Dispose -= preDisposeDelegate;
                    return true;
                }
                return false;
            }
            disposable.PreDispose -= preDisposeDelegate;
            return true;
        }
        
        public static IEnumerable<FieldInfo> GetAllFields(this Type type, BindingFlags flags = BindingFlags.Default)
        {
            var fields = type.GetFields(flags);
            foreach (var field in fields)
            {
                yield return field;
            }

            if (type.BaseType != null)
            {
                foreach (var field in GetAllFields(type.BaseType, flags))
                {
                    yield return field;
                }
            }
        }
    }
}