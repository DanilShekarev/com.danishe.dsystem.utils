using System;
using DSystemUtils.Dynamic.DI.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DSystemUtils.Dynamic.Editor
{
    [CustomPropertyDrawer(typeof(SelectClass))]
    public class SelectClassDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            UpdateClassData(root, property);
            
            root.Add(new PropertyField(property));

            return root;
        }
        
        private void UpdateClassData(VisualElement root, SerializedProperty serializedProp)
        {
            var popup = new PopupField<CompileTimeData.PairTypeAttr>("Class type");
            var testString = serializedProp.managedReferenceFieldTypename.Replace("Assembly-CSharp ", "");
            var propType = CompileTimeData.Assembly.GetType(testString);
            var obj = serializedProp.managedReferenceValue;
            var currentType = obj?.GetType();
            popup.formatListItemCallback = val =>
            {
                if (val.Attribute == null || string.IsNullOrEmpty(val.Attribute.Category))
                {
                    if (val.Type == null)
                    {
                        return "None";
                    }
                    return val.Type.Name;
                }

                string ret = $"{val.Attribute.Category}/{val.Type.Name}";
                return ret;
            };
            popup.formatSelectedValueCallback = DrawName;
            popup.RegisterValueChangedCallback(OnClassChanged);
            root.Add(popup);
            
            CompileTimeData.DrawSelectType(popup, currentType, propType);
            
            void OnClassChanged(ChangeEvent<CompileTimeData.PairTypeAttr> changeEvent)
            {
                var newVal = changeEvent.newValue;
                
                if (newVal.Type != null)
                    serializedProp.managedReferenceValue = Activator.CreateInstance(newVal.Type);
                else
                    serializedProp.managedReferenceValue = null;
            
                serializedProp.serializedObject.ApplyModifiedProperties();
            }
        }
        
        private string DrawName(CompileTimeData.PairTypeAttr attr)
        {
            if (attr.Type == null)
                return "None";
            return attr.Type.Name;
        }
    }
}