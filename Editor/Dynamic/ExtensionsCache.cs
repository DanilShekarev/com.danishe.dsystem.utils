using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Callbacks;

namespace DSystemUtils.Dynamic.Editor
{
    public static class ExtensionsCache
    {
        public static IReadOnlyDictionary<Type, List<MethodInfo>> ExtensionsMethods => _index;
        private static Dictionary<Type, List<MethodInfo>> _index = new();
        
        [DidReloadScripts]
        private static void OnReload()
        {
            var extensionClasses = Assembly.Load("Assembly-CSharp").GetTypes()
                .Where(t => t.IsStaticClass());

            foreach (var cls in extensionClasses)
            {
                var methods = cls.GetMethods()
                    .Where(m => m.IsExtensionMethod());

                foreach (var method in methods)
                {
                    var targetType = method.GetParameters()[0].ParameterType;
                    if (!_index.ContainsKey(targetType))
                    {
                        _index[targetType] = new List<MethodInfo>();
                    }
                    _index[targetType].Add(method);
                }
            }
        }
        
        private static bool IsStaticClass(this Type type)
        {
            return type.IsAbstract && type.IsSealed && type.IsClass;
        }
        
        private static bool IsExtensionMethod(this MethodInfo method)
        {
            return method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false);
        }
    }
}