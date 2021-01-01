using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Commons;
using UnityEditor;
using UnityEngine;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor
{
    public abstract class ExtendedDrawer : PropertyDrawer
    {
        protected static readonly float lineHeight = EditorGUIUtility.singleLineHeight + 3f; 
        
        private readonly IDictionary<string, bool> _showMap = new Dictionary<string, bool>();
        private readonly IDictionary<string, int> _showTab = new Dictionary<string, int>();

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
        
        protected void TabArea(string title, params TabItem[] items) 
        {
            LabeledArea(title, false, () =>
            {
                if (!_showTab.ContainsKey(title))
                {
                    _showTab.Add(title, 0);
                }

                _showTab[title] = GUILayout.Toolbar(_showTab[title], items.Select(x => x.Title).ToArray());
                items[_showTab[title]]?.OnGUI?.Invoke();
            });
        }
        
        protected static Rect CalculateNext(Rect rect, uint lines = 1)
        {
            return new Rect(rect.x, rect.y + lineHeight * lines, rect.width, lineHeight);
        }
    }
}