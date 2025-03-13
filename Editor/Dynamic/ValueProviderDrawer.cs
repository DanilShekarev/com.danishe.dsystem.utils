using System;
using System.Collections.Generic;
using System.Reflection;
using DSystem;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DSystemUtils.Dynamic.Editor
{
    [CustomPropertyDrawer(typeof(ValueProvider<>))]
    public class ValueProviderDrawer : PropertyDrawer 
    {
        private string[] GetMethodsNames(Object target, Type propertyType)
        {
            if (target == null)
                return new [] {"None"};
            
            var type = target.GetType();
            var retList = new List<string>();
            ReqFindMethods(type, retList, propertyType);
            return retList.ToArray();
        }
        
        private void ReqFindMethods(Type type, List<string> propNames, Type propertyType)
        {
            if (type == null || type == typeof(Object) || type == typeof(MonoBehaviour) || type == typeof(DBehaviour))
            {
                return;
            }
            var props = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in props)
            {
                if (method.ReturnType != propertyType) continue;
                propNames.Add(method.Name);
            }
            ReqFindMethods(type.BaseType, propNames, propertyType);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetProp = property.FindPropertyRelative("target");
            var methodProp = property.FindPropertyRelative("methodName");
            var defProp = property.FindPropertyRelative("defaultValue");
            
            EditorGUILayout.Space();
            EditorGUILayout.ObjectField(targetProp);
            var type = GetType(property);
            var methods = GetMethodsNames(targetProp.objectReferenceValue, type);
            int index = Array.IndexOf(methods, methodProp.stringValue);
            if (index == -1) index = 0;
            var newIndex = EditorGUILayout.Popup("Method", index, methods);
            EditorGUILayout.PropertyField(defProp);
            
            if (index == newIndex) return;
            
            methodProp.stringValue = methods[newIndex];
            
            property.serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space();
        }
        
        public static Type GetType(SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo fi = parentType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fi == null) return null;
            return fi.FieldType.GenericTypeArguments[0];
        }
    }   
}