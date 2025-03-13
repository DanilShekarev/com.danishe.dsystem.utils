using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystemUtils.Dynamic.DI.Editor
{
    public static class CompileTimeData
    {
        public static Assembly Assembly { get; private set; }
        
        public static PairTypeAttr[] ReflectionAttrClasses { get; private set; }
        
        public struct PairTypeAttr
        {
            public Type Type;
            public ReflectionClassAttribute Attribute;

            public PairTypeAttr(Type type, ReflectionClassAttribute attribute)
            {
                Type = type;
                Attribute = attribute;
            }
        }
        
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            Assembly = Assembly.Load("Assembly-CSharp");
            ReflectionAttrClasses = Assembly.GetTypes().Where(t => t.IsClass)
                .Select(t => new PairTypeAttr(t, t.GetCustomAttribute<ReflectionClassAttribute>()))
                .Where(p => p.Attribute != null)
                .ToArray();
        }
        
        public static void DrawSelectType(PopupField<PairTypeAttr> popupField, Type type, Type fieldType)
        {
            List<PairTypeAttr> choises = new List<PairTypeAttr>();
            choises.Add(default(PairTypeAttr));
            choises.AddRange(ReflectionAttrClasses.Where(p => fieldType.IsAssignableFrom(p.Type)));
            
            var currentIndex = choises.FindIndex(c => c.Type == type);

            popupField.choices = choises;
            popupField.index = Mathf.Max(currentIndex, 0);
        }
    }
}