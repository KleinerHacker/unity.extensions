using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;
using PcSoft.UnityInput._90_Scripts._90_Editor.Utils.Extensions;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using InputValue = PcSoft.UnityInput._90_Scripts._00_Runtime.Assets.InputValue;

namespace PcSoft.UnityInput._90_Scripts._90_Editor.Assets
{
    [CustomEditor(typeof(InputPreset))]
    public sealed class InputPresetEditor : ExtendedEditor._90_Scripts._90_Editor.ExtendedEditor
    {
        private SerializedProperty _itemsProperty;

        private Vector2 _scrollPositionItems = Vector2.zero;
        private Vector2 _scrollPositionContent = Vector2.zero;
        private TreeViewState _treeViewState = new TreeViewState();

        private InputItemTreeView _itemTreeView;

        private void OnEnable()
        {
            _itemsProperty = serializedObject.FindProperty("items");
            _itemTreeView = new InputItemTreeView(_treeViewState);
            _itemTreeView.UpdatePreset(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            BuildItemSelection();
            EditorGUILayout.Separator();
            BuildItemContent();

            serializedObject.ApplyModifiedProperties();
        }

        private void BuildItemSelection()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Input Items (" + _itemsProperty.arraySize + ")");
                if (GUILayout.Button("+", GUILayout.Width(50f)))
                {
                    OnAddRootItem();
                }

                if (GUILayout.Button("-", GUILayout.Width(50f)))
                {
                    var selectedInputItemProperty = _itemTreeView.GetSelectedInputItemProperty(_treeViewState);
                    OnRemoveSelectedItem(selectedInputItemProperty);
                }
            }
            GUILayout.EndHorizontal();
            _scrollPositionItems = GUILayout.BeginScrollView(_scrollPositionItems, GUILayout.MaxHeight(300f));
            {
                _itemTreeView.Reload();
                _itemTreeView.OnGUI(EditorGUILayout.GetControlRect(false, 300f));
            }
            GUILayout.EndScrollView();
        }

        private void BuildItemContent()
        {
            var inputItemProperty = _itemTreeView.GetSelectedInputItemProperty(_treeViewState);
            if (inputItemProperty == null)
                return;
            
            _scrollPositionContent = GUILayout.BeginScrollView(_scrollPositionContent, GUILayout.Height(400f));
            {
                BuildItemEditor(inputItemProperty);
            }
            GUILayout.EndScrollView();
        }

        private void BuildItemEditor(SerializedProperty property)
        {
            var nameProperty = property.FindPropertyRelative("name");
            var typeProperty = property.FindPropertyRelative("type");
            var valueProperty = property.FindPropertyRelative("value");
            var behaviorProperty = property.FindPropertyRelative("behavior");
            var fieldProperty = property.FindPropertyRelative("field");
            var actionsProperty = property.FindPropertyRelative("actions");
            var subItemsProperty = property.FindPropertyRelative("subItems");

            EditorGUILayout.PropertyField(nameProperty, new GUIContent("Name & Identifier"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(typeProperty, new GUIContent("Type"));
            EditorGUILayout.PropertyField(valueProperty, new GUIContent("Value Type"));
            if ((InputValue) valueProperty.intValue == InputValue.Button)
            {
                EditorGUILayout.PropertyField(behaviorProperty, new GUIContent("Behavior"));
            }
            EditorGUILayout.Separator();

            (string name, string displayName)[] fields = ExtractFields((InputType) typeProperty.intValue, (InputValue) valueProperty.intValue);
            var curField = fieldProperty.stringValue;
            var curFieldIndex = fields.Select(x => x.name).ToList().IndexOf(curField);
            var newFieldIndex = EditorGUILayout.Popup(curFieldIndex, fields.Select(x => x.displayName).ToArray());
            var newField = newFieldIndex < 0 || newFieldIndex >= fields.Length ? null : fields[newFieldIndex].name;
            fieldProperty.stringValue = newField;

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(actionsProperty, new GUIContent("Actions"));
            EditorGUILayout.Separator();
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Sub Item", GUILayout.Width(150f)))
                {
                    OnAddSubItem(subItemsProperty);
                }
            }
            GUILayout.EndHorizontal();
        }
        
        private static (string, string)[] ExtractFields(InputType type, InputValue value)
        {
            var device = type.GetFitDevice();
            return device.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => value.GetFitType().IsAssignableFrom(x.PropertyType))
                .Where(x => x.GetMethod.GetParameters().Length == 0)
                .Select(x =>
                {
                    try
                    {
                        return (x.Name, ((InputControl) x.GetValue(device)).displayName);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(x.Name + ": " + x.PropertyType.FullName + ": " + x.GetMethod + ": " + x.DeclaringType);
                        throw;
                    }
                })
                .OrderBy(x => x.displayName)
                .ToArray();
        }

        private void OnAddRootItem()
        {
            _itemsProperty.InsertArrayElementAtIndex(_itemsProperty.arraySize);
        }

        private void OnAddSubItem(SerializedProperty subItemsProperty)
        {
            subItemsProperty.InsertArrayElementAtIndex(subItemsProperty.arraySize);
        }

        private void OnRemoveSelectedItem(SerializedProperty selectedInputItemProperty)
        {
            if (EditorUtility.DisplayDialog("Remove Input Item", "You are sure to delete selected input item (and all sub items)?", "Yes", "No"))
            {
                var path = selectedInputItemProperty.propertyPath;
                var index = int.Parse(path.Substring(path.LastIndexOf("[") + 1, path.LastIndexOf("]") - (path.LastIndexOf("[") + 1)));
                var parentPath = path.Substring(0, path.LastIndexOf(".Array"));
                var property = serializedObject.FindProperty(parentPath);
                
                property.DeleteArrayElementAtIndex(index);
            }
        }
    }
}