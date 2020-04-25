using System;
using System.Collections.Generic;
using UnityEditor;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor
{
    public abstract class ExtendedDrawer : PropertyDrawer
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
                _showMap.Add(label, false);
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
    }
}