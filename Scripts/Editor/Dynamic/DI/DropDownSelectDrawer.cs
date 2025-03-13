using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DSystemUtils.Dynamic.DI.Editor
{
    [CustomPropertyDrawer(typeof(DropDownSelect))]
    public class DropDownSelectDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            var curVal = property.objectReferenceValue;

            var propType = fieldInfo.FieldType;
            
            var scriptables = Resources.LoadAll("", propType);

            PopupField<Object> popupField = new PopupField<Object>
            {
                label = property.displayName,
                formatListItemCallback = GetName,
                formatSelectedValueCallback = GetName
            };
            popupField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                property.objectReferenceValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });
            popupField.choices = scriptables.ToList();

            int curIndex = 0;
            if (curVal != null)
            {
                curIndex = Array.IndexOf(scriptables, curVal);
            }
            else
            {
                if (scriptables.Length > 0)
                {
                    property.objectReferenceValue = scriptables[0];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            
            popupField.index = curIndex;
            
            root.Add(popupField);
            
            return root;
        }

        private string GetName(Object arg)
        {
            if (arg == null)
            {
                return "None";
            }
            return arg.name;
        }
    }
}