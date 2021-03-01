using System.Collections.Generic;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace PcSoft.UnityInput._90_Scripts._90_Editor.Assets
{
    public class InputItemTreeView : TreeView
    {
        private SerializedObject _presetProperty;

        public InputItemTreeView(TreeViewState state) : base(state)
        {
        }

        public InputItemTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public void UpdatePreset(SerializedObject preset)
        {
            _presetProperty = preset;
            Reload();
        }

        public SerializedProperty GetSelectedInputItemProperty(TreeViewState state)
        {
            return state.GetSelectedInputItemProperty(rootItem);
        }

        protected override TreeViewItem BuildRoot()
        {
            var rootItem = new TreeViewItem(0, -1, "Input Preset");

            if (_presetProperty == null)
                return rootItem;

            var itemProperties = _presetProperty.FindProperties("items");

            var id = 0;
            BuildNextLevel(itemProperties, rootItem, ref id);
            return rootItem;
        }

        private void BuildNextLevel(SerializedProperty[] itemProperties, TreeViewItem rootItem, ref int runningId, int depth = 0)
        {
            rootItem.children = new List<TreeViewItem>();
            foreach (var itemProperty in itemProperties)
            {
                runningId++;
                var name = BuildString(itemProperty);
                var subItemProperties = itemProperty.FindPropertiesRelative("subItems");

                var subItem = new InputItemTreeViewItem {id = runningId, depth = depth, displayName = name, InputItemProperty = itemProperty};
                BuildNextLevel(subItemProperties, subItem, ref runningId, depth + 1);

                rootItem.children.Add(subItem);
            }
        }

        private string BuildString(SerializedProperty property)
        {
            var name = property.FindPropertyRelative("name").stringValue;
            var type = (InputType) property.FindPropertyRelative("type").intValue;
            var value = (InputValue) property.FindPropertyRelative("value").intValue;
            var behavior = (InputBehavior) property.FindPropertyRelative("behavior").intValue;
            var field = property.FindPropertyRelative("field").stringValue;

            return $"{type} / {value} {(value == InputValue.Button ? " / " + behavior : "")} ({field}) {(string.IsNullOrWhiteSpace(name) ? "" : " / " + name)}";
        }
    }

    public class InputItemTreeViewItem : TreeViewItem
    {
        public SerializedProperty InputItemProperty { get; set; }
    }

    public static class TreeViewStateExtensions
    {
        public static SerializedProperty GetSelectedInputItemProperty(this TreeViewState state, TreeViewItem rootItem)
        {
            if (state.lastClickedID == 0)
                return null;

            return FindNext(state.lastClickedID, rootItem.children.OfType<InputItemTreeViewItem>().ToArray());
        }

        private static SerializedProperty FindNext(int id, InputItemTreeViewItem[] items)
        {
            foreach (var item in items)
            {
                if (item.id == id)
                    return item.InputItemProperty;

                var foundInputItemProperty = FindNext(id, item.children.OfType<InputItemTreeViewItem>().ToArray());
                if (foundInputItemProperty != null)
                    return foundInputItemProperty;
            }

            return null;
        }
    }
}