using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DSystem;
using DSystemUtils.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using MethodInfo = System.Reflection.MethodInfo;
using Object = UnityEngine.Object;

namespace DSystemUtils.Dynamic.Editor
{
    [CustomPropertyDrawer(typeof(DEvent<>))]
    public class DEventDrawer : PropertyDrawer
    {
        private SerializedProperty _props;
        private SerializedProperty _main;
        private ReorderableList _reorderableList;
        private Type _genericType;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _props = property.FindPropertyRelative("events");
            _main = property;
            
            EditorGUI.BeginChangeCheck();

            ReorderableList list = GetList();
            
            list.DoLayoutList();
            
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        
        public static Type GetType(SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo fi = parentType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (fi == null) return null;
            return fi.FieldType.GenericTypeArguments[0];
        }

        private void DrawHeaderCallback(Rect rect)
        {
            _genericType = GetType(_main);
            var argsStr = $" ({_genericType.Name})";
            EditorGUI.LabelField(rect, $"{_main.displayName}{argsStr}");
        }

        private ReorderableList GetList()
        {
            if (_reorderableList == null || _reorderableList.serializedProperty.serializedObject != _props.serializedObject)
            {
                _reorderableList = new ReorderableList(_props.serializedObject, _props, 
                    true, true, true, true);
                
                _reorderableList.drawElementCallback = DrawElementCallback;
                _reorderableList.drawHeaderCallback = DrawHeaderCallback;
                _reorderableList.elementHeightCallback = ElementHeightCallback;
            }
            
            return _reorderableList;
        }

        private float ElementHeightCallback(int index)
        {
            float defaultHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            var element = _props.GetArrayElementAtIndex(index);
            var targetProp = element.FindPropertyRelative("target");
            var target = targetProp.objectReferenceValue;

            if (target == null)
                return defaultHeight;
            
            var methodName = element.FindPropertyRelative("methodName").stringValue;
            if (string.IsNullOrEmpty(methodName) || methodName == "No Function")
                return defaultHeight;

            methodName = methodName.Substring(methodName.IndexOf('(') + 1);
            methodName = methodName.TrimEnd(')');
            var parameters = methodName.Split(", ");

            return defaultHeight * (parameters.Length + 1);
        }
        
        private Rect[] GetRowRects(Rect rect)
        {
            Rect[] rowRects = new Rect[2];
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            Rect rect1 = rect;
            rect1.width *= 0.3f;
            Rect rect3 = new Rect(rect)
            {
                xMin = rect1.xMax + 5f
            };
            rowRects[0] = rect1;
            rowRects[1] = rect3;
            return rowRects;
        }

        private readonly Type[] _excludedClasses = 
        {
            typeof(MonoBehaviour), typeof(DBehaviour), typeof(Object), typeof(Component)
        };

        private string GetParamsNames(Type[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return "";
            
            StringBuilder sb = new StringBuilder();
            
            sb.Append(" (");
            
            for (int index = 0; index < parameters.Length; ++index)
            {
                sb.Append(parameters[index].Name);
                if (index < parameters.Length - 1)
                    sb.Append(", ");
            }
            
            sb.Append(")");
            
            return sb.ToString();
        }

        private string[] GetMethodsNames(Object target, out MethodInfo[] methods)
        {
            if (target == null)
            {
                methods = null;
                return new [] {"No Function"};   
            }
            
            var type = target.GetType();
            var retList = new List<string> { "No Function" };
            var methodList = new List<MethodInfo> { null };
            var props = type.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance 
                                                                  | BindingFlags.Public);
            foreach (var method in props)
            {
                if (method.IsSpecialName) continue;
                if (method.ReturnType != typeof(void)) continue;
                if (_excludedClasses.Contains(method.DeclaringType)) continue;
                var parameters = GetMethodParametersType(method);
                retList.Add($"{method.Name}{GetParamsNames(parameters)}");
                methodList.Add(method);
            }

            methods = methodList.ToArray();
            return retList.ToArray();
        }

        private Type[] GetMethodParametersType(MethodInfo method)
        {
            return method.GetParameters().Select(p => p.ParameterType).ToArray();
        }
        
        private Rect _currentElementRect;

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            _currentElementRect = rect;
            
            var element = _props.GetArrayElementAtIndex(index);
            var targetProp = element.FindPropertyRelative("target");
            var target = targetProp.objectReferenceValue;

            var rects = GetRowRects(rect);
            
            EditorGUI.ObjectField(rects[0], targetProp, new GUIContent());
            CheckMethod(rects[1], target, element);
        }

        private void CheckMethod(Rect rect, Object target, SerializedProperty element)
        {
            var methodPorp = element.FindPropertyRelative("methodName");
            //var methodSign = element.FindPropertyRelative("methodSignature");
            var methodParams = element.FindPropertyRelative("methodsParamsTypes");
            var serializedParams = element.FindPropertyRelative("serializedValues");
            var serializedObjectsParams = element.FindPropertyRelative("serializedObjects");
            var methodsNames = GetMethodsNames(target, out var methods);
            int methodIndex = Array.IndexOf(methodsNames, methodPorp.stringValue);
            if (methodIndex == -1) methodIndex = 0;
            
            EditorGUI.BeginChangeCheck();
            methodIndex = EditorGUI.Popup(rect, methodIndex, methodsNames);

            if (EditorGUI.EndChangeCheck())
            {
                methodPorp.stringValue = methodsNames[methodIndex];
                if (methodIndex != 0)
                {
                    var paramsTypes = methods[methodIndex].GetParameters();
                    methodParams.arraySize = paramsTypes.Length;
                    for (int i = 0; i < paramsTypes.Length; i++)
                    {
                        var el = methodParams.GetArrayElementAtIndex(i);
                        el.stringValue = paramsTypes[i].ParameterType.FullName;
                    }
                }
                //methodSign.stringValue = methods[methodIndex].ToString();
                else
                    methodParams.arraySize = 0;
                //methodSign.stringValue = "";
            }

            if (methodIndex != 0)
                DrawMethodProperties(methods[methodIndex], serializedParams, serializedObjectsParams, element);
            else
                serializedParams.arraySize = 0;
        }
        
        private Rect[] GetParametersRowRects(Rect rect)
        {
            const float spacing = 5f;
            
            Rect[] rowRects = new Rect[4];
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            Rect rect1 = new Rect(rect);
            rect1.width *= 0.3f;
            var rect2 = new Rect(rect);
            rect2.xMin = rect1.xMax + spacing;
            
            var rect3 = new Rect(rect);
            rect3.width *= 0.15f;
            rect3.position = new Vector2(rect1.xMax + spacing, rect3.position.y);
            var rect4 = new Rect(rect);
            rect4.xMin = rect3.xMax + spacing;
            
            rowRects[0] = rect1;
            rowRects[1] = rect2;
            rowRects[2] = rect3;
            rowRects[3] = rect4;
            return rowRects;
        }

        private void DrawMethodProperties(MethodInfo method, SerializedProperty serializedParams, 
            SerializedProperty objectsParams, SerializedProperty element)
        {
            var parameters = method.GetParameters();
            var properties = _genericType.GetProperties();
            var propsNamesProp = element.FindPropertyRelative("propertyNames");
            
            List<string> tempNames = new List<string>();
            
            serializedParams.arraySize = parameters.Length;
            objectsParams.arraySize = parameters.Length;
            propsNamesProp.arraySize = parameters.Length;

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                tempNames.Clear();
                tempNames.Add("None");

                _currentElementRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var rects = GetParametersRowRects(_currentElementRect);

                EditorGUI.LabelField(rects[0], parameter.Name);
                var parType = parameter.ParameterType;
                if (_genericType.IsConvertibleTo(parType, true))
                {
                    tempNames.Add("this");
                }
                var assignableProps = properties
                    .Where(p => parType.IsAssignableFrom(p.PropertyType))
                    .Select(p => p.Name);
                tempNames.AddRange(assignableProps);

                var serProp = serializedParams.GetArrayElementAtIndex(i);
                var objProp = objectsParams.GetArrayElementAtIndex(i);

                if (tempNames.Count > 1)
                {
                    EditorGUI.BeginChangeCheck();
                    var propNames = tempNames.ToArray();
                    var propNameProp = propsNamesProp.GetArrayElementAtIndex(i);
                    var propIndex = Array.IndexOf(propNames, propNameProp.stringValue);
                    if (propIndex == -1)
                        propIndex = 0;
                    
                    propIndex = EditorGUI.Popup(propIndex == 0 ? rects[2] : rects[1], propIndex, propNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        propNameProp.stringValue = propNames[propIndex];
                    }

                    if (propIndex == 0)
                    {
                        DrawSerializedProp(rects[3], parType, serProp, objProp, parameter);
                        propNameProp.stringValue = null;
                    }
                }
                else
                {
                    DrawSerializedProp(rects[1], parType, serProp, objProp, parameter);
                }
            }
        }

        private void DrawSerializedProp(Rect rect, Type type, SerializedProperty dataProp, SerializedProperty objProp,
            ParameterInfo parameterInfo)
        {
            string data = dataProp.stringValue;
            Object objRef = objProp.objectReferenceValue;
            if (EditorUtils.DrawSerializedProp(rect, type, ref data, ref objRef))
            {
                dataProp.stringValue = data;
                objProp.objectReferenceValue = objRef;
            }
            else
            {
                EditorGUI.LabelField(rect, $"{parameterInfo.Name} is not serializable!");
            }
        }
    }
}