using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using DSystem;
using DSystemUtils.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DSystemUtils.Dynamic.Editor
{
    public class EventDataDrawer
    {
        private SerializedProperty _property;
        private Type _genericType;
        private VisualElement _content;
        
        public VisualElement CreatePropertyGUI(SerializedProperty property, Type genericType)
        {
            _genericType = genericType;
            _property = property;
            
            VisualElement root = new VisualElement();

            root.Add(MakeHeader(property));
            
            _content = new VisualElement();
            RefreshContent();
            
            root.Add(_content);
            return root;
        }
        
        #region Header
        
        private VisualElement MakeHeader(SerializedProperty property)
        {
            VisualElement header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;

            var targetProp = property.FindPropertyRelative("target");

            var selectMemberField = DrawTargetMember(targetProp);
            var targetField = DrawTargetSelect(targetProp, selectMemberField);
            
            header.Add(targetField);
            header.Add(selectMemberField);
            
            return header;
        }

        private VisualElement DrawTargetSelect(SerializedProperty targetProp, PopupField<PopupPair> popupField)
        {
            ObjectField objectField = new ObjectField()
            {
                name = "target",
                style = { width = new Length(30, LengthUnit.Percent) }
            };
            objectField.BindProperty(targetProp);
            
            objectField.RegisterValueChangedCallback(evt => RefreshPopup(popupField, evt.newValue));
            
            return objectField;
        }
        
        private struct PopupPair
        {
            public int Index;
            public Object Target;
            public MethodInfo Method;
        }

        private PopupField<PopupPair> DrawTargetMember(SerializedProperty targetProp)
        {
            PopupField<PopupPair> popupField = new PopupField<PopupPair>()
            {
                name = "methodPopup",
                style = { width = new Length(70, LengthUnit.Percent) },
            };
            popupField.RegisterValueChangedCallback(OnChangeTargetMethod);
            popupField.formatSelectedValueCallback = FormatSelectedMethod;
            popupField.formatListItemCallback = FormatSelectedValueCallback;
            
            RefreshPopup(popupField, targetProp.objectReferenceValue);
            
            return popupField;
        }

        private void RefreshPopup(PopupField<PopupPair> popupField, Object target)
        {
            popupField.choices = GetTargetMembers(target);
            popupField.index = GetIndex(popupField.choices, _property, target);
        }
        
        private string GetParamsNames(Type[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return "";
            
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
        
        private Type[] GetMethodParametersType(MethodInfo method)
        {
            return method.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        private string FormatSelectedMethod(PopupPair methodInfo)
        {
            if (methodInfo.Method == null)
                return "None";
            return $"{methodInfo.Method.Name}{GetParamsNames(GetMethodParametersType(methodInfo.Method))}";
        }
        
        private string FormatSelectedValueCallback(PopupPair methodInfo)
        {
            if (methodInfo.Method == null)
                return "None";
            if (methodInfo.Index == 0)
                return $"{methodInfo.Target}/{methodInfo.Method.Name}{GetParamsNames(GetMethodParametersType(methodInfo.Method))}";
            return $"{methodInfo.Target} ({methodInfo.Index})/{methodInfo.Method.Name}{GetParamsNames(GetMethodParametersType(methodInfo.Method))}";
        }
        
        private void OnChangeTargetMethod(ChangeEvent<PopupPair> evt)
        {
            var targetProp = _property.FindPropertyRelative("target");
            var targetMethodProp = _property.FindPropertyRelative("targetMember");
            var typeNameProp = targetMethodProp.FindPropertyRelative("_declaringTypeName");
            var methodNameProp = targetMethodProp.FindPropertyRelative("_memberName");
            var parametersNamesProp = targetMethodProp.FindPropertyRelative("_parameterTypeNames");
                
            var pair = evt.newValue;
            var method = pair.Method;
            if (method == null)
            {
                typeNameProp.stringValue = string.Empty;
                methodNameProp.stringValue = string.Empty;
                parametersNamesProp.arraySize = 0;
            }
            else
            {
                targetProp.objectReferenceValue = pair.Target;
                typeNameProp.stringValue = method.DeclaringType.AssemblyQualifiedName;
                methodNameProp.stringValue = method.Name;
                
                var parameters = method.GetParameters();
                parametersNamesProp.arraySize = parameters.Length;
                for (int i = 0; i < parameters.Length; i++)
                {
                    parametersNamesProp.GetArrayElementAtIndex(i).stringValue = parameters[i].ParameterType.AssemblyQualifiedName;
                }
            }
            RefreshContent();
                
            _property.serializedObject.ApplyModifiedProperties();
        }
        
        private int GetIndex(List<PopupPair> methods, SerializedProperty property, Object target)
        {
            var method = GetMethod(property.FindPropertyRelative("targetMember"));
            if (method == null)
                return 0;
            int index = methods.FindIndex(p =>
            {
                if (p.Method == null)
                    return false;
                return p.Method.MetadataToken == method.MetadataToken && p.Target == target;
            });
            return index;
        }
        
        private MethodInfo GetMethod(SerializedProperty targetMember)
        {
            var typeNameProp = targetMember.FindPropertyRelative("_declaringTypeName");
            var methodNameProp = targetMember.FindPropertyRelative("_memberName");
            var parametersNamesProp = targetMember.FindPropertyRelative("_parameterTypeNames");

            string[] paramsTypes = new string[parametersNamesProp.arraySize];

            for (int i = 0; i < paramsTypes.Length; i++)
            {
                paramsTypes[i] = parametersNamesProp.GetArrayElementAtIndex(i).stringValue;
            }

            return SerializableMember.GetMethod(typeNameProp.stringValue, methodNameProp.stringValue, paramsTypes);
        }
        
        private readonly Type[] _excludedClasses = 
        {
            typeof(MonoBehaviour), typeof(DBehaviour), typeof(Object), typeof(Component)
        };
        
        private List<PopupPair> GetTargetMembers(Object target)
        {
            var methodList = new List<PopupPair> { new () };
            if (target == null)
                return methodList;

            GameObject obj = null;
            if (target is GameObject go)
                obj = go;
            else if (target is Component comp)
                obj = comp.gameObject;

            if (obj != null)
            {
                var components = obj.GetComponents<Component>();
                Dictionary<Type, int> indexPairs = new ();
                foreach (var component in components)
                {
                    var type = component.GetType();
                    if (!indexPairs.TryAdd(type, 0))
                        indexPairs[type]++;
                    var index = indexPairs[type];
                    methodList.AddRange(GetMethods(component, index));
                }
            }
            else
            {
                methodList.AddRange(GetMethods(target, 0));
            }
            return methodList;
        }

        private IEnumerable<PopupPair> GetMethods(Object target, int index)
        {
            var type = target.GetType();
            var props = type.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance 
                                                                  | BindingFlags.Public).AsEnumerable();
                
            if (ExtensionsCache.ExtensionsMethods.TryGetValue(type, out var extensionMethods))
            {
                props = props.Concat(extensionMethods);
            }
            foreach (var method in props)
            {
                if (method.ReturnType != typeof(void))
                    continue;
                if (_excludedClasses.Contains(method.DeclaringType))
                    continue;
                yield return new PopupPair { Target = target, Method = method, Index = index };
            }
        }
        #endregion

        #region Content

        private struct CustomSelectMember
        {
            public bool This;
            public MethodInfo Method;
        }
        
        private void RefreshContent()
        {
            _content.Clear();
            
            var method = GetMethod(_property.FindPropertyRelative("targetMember"));
            if (method == null)
                return;
            
            var valuesParams = _property.FindPropertyRelative("serializedValues");
            var serializedObjectsParams = _property.FindPropertyRelative("serializedObjects");
            
            var dataMembersProp = _property.FindPropertyRelative("dataMembers");
            var thisUseProps = _property.FindPropertyRelative("useThis");
            
            var parameters = method.GetParameters();

            {
                bool changed = false;
                var paramsCount = parameters.Length;
                changed |= UpdateArraySize(valuesParams, paramsCount);
                changed |= UpdateArraySize(serializedObjectsParams, paramsCount);
                changed |= UpdateArraySize(dataMembersProp, paramsCount);
                changed |= UpdateArraySize(thisUseProps, paramsCount);
                if (changed)
                    _property.serializedObject.ApplyModifiedProperties();
            }
            
            bool isExtensionMethod = method.IsDefined(typeof(ExtensionAttribute), true);
            
            for (var i = isExtensionMethod ? 1 : 0; i < parameters.Length; i++)
            {
                List<CustomSelectMember> selectMembers = new() { default };

                var parameter = parameters[i];
                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;

                Label label = new Label(parameter.Name)
                {
                    style = { width = new Length(30, LengthUnit.Percent), unityTextAlign = TextAnchor.MiddleRight}
                };
                
                var parType = parameter.ParameterType;
                if (parType.IsAssignableFrom(_genericType))
                {
                    selectMembers.Add(new CustomSelectMember {This = true});
                }
                selectMembers.AddRange(GetMethods(_genericType, parType));
                
                PopupField<CustomSelectMember> field = new ()
                {
                    choices = selectMembers,
                    formatSelectedValueCallback = FormatSelectMember,
                    formatListItemCallback = FormatSelectMember
                };
                var memberProp = dataMembersProp.GetArrayElementAtIndex(i);
                var thisUseProp = thisUseProps.GetArrayElementAtIndex(i);
                field.index = GetIndex(memberProp, thisUseProp, selectMembers);
                
                var propContent = DrawProperty(valuesParams.GetArrayElementAtIndex(i),
                    serializedObjectsParams.GetArrayElementAtIndex(i), parameter);
                
                field.RegisterValueChangedCallback(GetChangeEvent(memberProp, thisUseProp, propContent));

                row.Add(label);
                row.Add(field);
                
                if (propContent != null)
                {
                    propContent.style.flexGrow = 10;
                    propContent.visible = field.index == 0;   
                    row.Add(propContent);
                }

                _content.Add(row);
            }
        }

        private bool UpdateArraySize(SerializedProperty property, int newSize)
        {
            if (property.arraySize == newSize)
                return false;
            property.arraySize = newSize;
            return true;
        }

        private int GetIndex(SerializedProperty memberProp, SerializedProperty thisUseProp, List<CustomSelectMember> members)
        {
            if (thisUseProp.boolValue)
                return members.FindIndex(m => m.This);
            
            var method = GetMethod(memberProp);
            if (method == null)
                return 0;

            return members.FindIndex(m => m.Method == method);
        }

        private EventCallback<ChangeEvent<CustomSelectMember>> GetChangeEvent(SerializedProperty memberProp, 
            SerializedProperty thisUseProp, VisualElement inputField)
        {
            return evt =>
            {
                var typeNameProp = memberProp.FindPropertyRelative("_declaringTypeName");
                var methodNameProp = memberProp.FindPropertyRelative("_memberName");
                var parametersNamesProp = memberProp.FindPropertyRelative("_parameterTypeNames");
                
                var newMember = evt.newValue;
                
                if (newMember.Method == null)
                {
                    typeNameProp.stringValue = string.Empty;
                    methodNameProp.stringValue = string.Empty;
                    parametersNamesProp.arraySize = 0;
                    thisUseProp.boolValue = newMember.This;
                    if (inputField != null)
                        inputField.visible = !newMember.This;
                } else
                {
                    typeNameProp.stringValue = newMember.Method.DeclaringType.AssemblyQualifiedName;
                    methodNameProp.stringValue = newMember.Method.Name;
                    if (inputField != null)
                        inputField.visible = false;
                    
                    var parameters = newMember.Method.GetParameters();
                    parametersNamesProp.arraySize = parameters.Length;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parametersNamesProp.GetArrayElementAtIndex(i).stringValue = parameters[i].ParameterType.AssemblyQualifiedName;
                    }
                }
                
                _property.serializedObject.ApplyModifiedProperties();
            };
        }

        private string FormatSelectMember(CustomSelectMember arg)
        {
            if (arg.Method == null && !arg.This)
                return "None";
            if (arg.This)
                return "This";
            return arg.Method.Name;
        }

        private IEnumerable<CustomSelectMember> GetMethods(Type type, Type needType)
        {
            var props = type.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance 
                                                                  | BindingFlags.Public).AsEnumerable();
                
            if (ExtensionsCache.ExtensionsMethods.TryGetValue(type, out var extensionMethods))
            {
                props = props.Concat(extensionMethods);
            }
            foreach (var method in props)
            {
                if (method.ReturnType != needType)
                    continue;
                if (_excludedClasses.Contains(method.DeclaringType))
                    continue;
                var prams = method.GetParameters();
                var extensionMethod = prams.Length == 1 && prams[0].ParameterType == type;
                if (prams.Length > 0 && !extensionMethod)
                    continue;
                yield return new CustomSelectMember { Method = method};
            }
        }
        
        private VisualElement DrawProperty(SerializedProperty valueProp, SerializedProperty objProp,
            ParameterInfo parameterInfo)
        {
            var type = parameterInfo.ParameterType; 
            if (!type.IsSerializable && !typeof(Object).IsAssignableFrom(type))
                return null;

            string data = valueProp.stringValue;
            Object objRef = objProp.objectReferenceValue;
            
            return EditorUtils.DrawSerializedProp(parameterInfo.ParameterType, data, objRef, SerDataChanged, ObjectChanged);
            
            void ObjectChanged(Object obj)
            {
                objProp.objectReferenceValue = obj;
                objProp.serializedObject.ApplyModifiedProperties();
            }
            
            void SerDataChanged(string obj)
            {
                valueProp.stringValue = obj;
                valueProp.serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}