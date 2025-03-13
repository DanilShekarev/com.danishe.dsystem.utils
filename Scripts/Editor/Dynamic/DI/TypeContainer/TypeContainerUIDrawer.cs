using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace DSystemUtils.Dynamic.DI.Editor
{
    [CustomPropertyDrawer(typeof(TypeContainer<>))]
    public class TypeContainerUIDrawer : PropertyDrawer
    {
        private VisualElement _helpBoxContainer;
        private HelpBox _helpBox;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            _helpBoxContainer = new VisualElement();
        
            var interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];
            var typeNameProp = property.FindPropertyRelative("_typeName");
            
            var currentType = GetTypeFromProperty(typeNameProp);
            
            var validTypes = GetValidTypes(interfaceType);
            
            var currentIndex = validTypes.IndexOf(currentType);
            if (currentIndex == -1) currentIndex = 0;
            
            var dropdown = new PopupField<Type>()
            {
                label = property.displayName,
                choices = validTypes,
                formatListItemCallback = FormatType,
                formatSelectedValueCallback = FormatType,
                index = currentIndex
            };

            dropdown.RegisterValueChangedCallback(evt => 
            {
                typeNameProp.stringValue = evt.newValue?.AssemblyQualifiedName ?? string.Empty;
                typeNameProp.serializedObject.ApplyModifiedProperties();
                UpdateValidationWarning(evt.newValue, validTypes);
            });

            container.Add(dropdown);
            container.Add(_helpBoxContainer);
            
            UpdateValidationWarning(currentType, validTypes);

            return container;
        }

        private void UpdateValidationWarning(Type currentType, List<Type> validTypes)
        {
            _helpBoxContainer.Clear();
        
            if (currentType != null && !validTypes.Contains(currentType))
            {
                _helpBox = new HelpBox(
                    $"Type {currentType.Name} is no longer valid!",
                    HelpBoxMessageType.Error
                );
                _helpBoxContainer.Add(_helpBox);
            }
        }

        private string FormatType(Type type) => type?.Name ?? "None";

        private List<Type> GetValidTypes(Type interfaceType) => 
            TypeCache.GetTypesDerivedFrom(interfaceType)
                .Where(t => t.IsClass && !t.IsAbstract)
                .OrderBy(t => t.FullName).Prepend(null)
                .ToList();

        private Type GetTypeFromProperty(SerializedProperty prop) => 
            !string.IsNullOrEmpty(prop.stringValue) ? 
                Type.GetType(prop.stringValue) : 
                null;
    }
}