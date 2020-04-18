using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PcSoft.ExtendedEditor._90_Scripts.Editor
{
    public abstract class ExtendedEditor : UnityEditor.Editor
    {
        private readonly IDictionary<string, bool> _showMap = new Dictionary<string, bool>();

        protected void IntentArea(Action action)
        {
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

            action();

            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
        }

        protected void LabeledArea(string label, Action action)
        {
            LabeledArea(label, true, action);
        }

        protected void LabeledArea(string label, bool intent, Action action)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            if (intent)
            {
                IntentArea(action);
            }
            else
            {
                action();
            }
        }

        protected void FoldoutArea(string label, Action action, bool toggleOnLabel = true)
        {
            FoldoutArea(label, true, action, toggleOnLabel);
        }

        protected void FoldoutArea(string label, bool intent, Action action, bool toggleOnLabel = true)
        {
            if (!_showMap.ContainsKey(label))
            {
                _showMap.Add(label, true);
            }

            _showMap[label] = EditorGUILayout.Foldout(_showMap[label], label, toggleOnLabel);
            if (!_showMap[label])
                return;

            if (intent)
            {
                IntentArea(action);
            }
            else
            {
                action();
            }
        }
        
        protected void HorizontalArea(Action<Rect> action)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            action(rect);
            EditorGUILayout.EndHorizontal();
        }

        protected ReorderableList BuildReordableList(SerializedProperty property, string title, float elementHeight,
            Action<SerializedProperty, Rect, int, bool, bool> elementDrawer, Action<SerializedProperty, int> adder = null)
        {
            return new ReorderableList(serializedObject, property)
            {
                draggable = true,
                displayAdd = true,
                displayRemove = true,
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, title),
                drawElementCallback = (rect, index, active, focused) =>
                {
                    // Get the currently to be drawn element from YourList
                    var element = property.GetArrayElementAtIndex(index);
                    elementDrawer(element, rect, index, active, focused);
                },
                elementHeight = elementHeight,
                onAddCallback = list =>
                {
                    var index = list.serializedProperty.arraySize;

                    // Since this method overwrites the usual adding, we have to do it manually:
                    // Simply counting up the array size will automatically add an element
                    list.serializedProperty.arraySize++;
                    list.index = index;
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    adder?.Invoke(element, index);
                }
            };
        }
    }
}