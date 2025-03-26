using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DSystemUtils.Dynamic.Editor
{
    [CustomPropertyDrawer(typeof(DEvent<>))]
    public class DEventDrawer : PropertyDrawer
    {
        private readonly Dictionary<int, EventDataDrawer> _drawers = new ();
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            var eventsProperty = property.FindPropertyRelative("events");
            
            Label label = new Label()
            {
                text = $"{property.displayName} ({fieldInfo.FieldType.GenericTypeArguments[0].Name})",
            };
            label.AddToClassList("unity-list-view__header");
            ListView listView = new ListView();
            listView.showAddRemoveFooter = true;
            listView.reorderMode = ListViewReorderMode.Animated;
            listView.reorderable = true;
            listView.showBorder = true;
            listView.showFoldoutHeader = false;
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            listView.showBoundCollectionSize = false;
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;
            listView.BindProperty(eventsProperty);
            
            listView.makeItem = MakeItem;
            listView.bindItem = (elRoot, index) =>
            {
                var drawer = new EventDataDrawer();
                elRoot.Clear();
                var eventProp = eventsProperty.GetArrayElementAtIndex(index);
                var ve = drawer.CreatePropertyGUI(eventProp, fieldInfo.FieldType.GenericTypeArguments[0]);
                elRoot.Add(ve);
                _drawers.Add(index, drawer);
            };
            listView.unbindItem = (elRoot, index) =>
            {
                _drawers.Remove(index);
            };
            
            root.Add(label);
            root.Add(listView);
            
            return root;
        }

        private VisualElement MakeItem()
        {
            return new VisualElement();
        }
    }
}