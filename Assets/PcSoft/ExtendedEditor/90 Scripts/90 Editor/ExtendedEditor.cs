using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Commons;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = System.Object;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor
{
    public abstract class ExtendedEditor : UnityEditor.Editor
    {
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

        protected void ArrayArea<T>(string title, SerializedProperty[] properties, T[] values, Func<T, SerializedProperty, bool> check, Func<T, SerializedProperty, string> titleFunc)
        {
            LabeledArea(title, () =>
            {
                foreach (var value in values)
                {
                    var property = properties.FirstOrDefault(item => check.Invoke(value, item));
                    if (property == null)
                        continue;
                    EditorGUILayout.PropertyField(property, new GUIContent(titleFunc.Invoke(value, property)), true);
                }
            });
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
    }

    public abstract class AutoEditor : ExtendedEditor
    {
        private readonly CustomGUIPosition _customGuiPosition;

        protected AutoEditor(CustomGUIPosition customGuiPosition = CustomGUIPosition.Bottom)
        {
            _customGuiPosition = customGuiPosition;
        }

        #region Builtin Methods

        protected virtual void OnEnable()
        {
            foreach (var field in GetType().GetRuntimeFields())
            {
                var attribute = field.GetCustomAttribute<SerializedPropertyReferenceAttribute>();
                if (attribute == null)
                    continue;

                if (field.FieldType.IsArray)
                {
                    field.SetValue(this, serializedObject.FindProperties(attribute.Name));
                }
                else
                {
                    field.SetValue(this, serializedObject.FindProperty(attribute.Name));
                }
            }
        }

        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_customGuiPosition == CustomGUIPosition.Top)
            {
                DoInspectorGUI();
            }

            var allFields = GetType().GetRuntimeFields()
                .Where(x => x.GetCustomAttribute<SerializedPropertyRepresentationAttribute>() != null)
                .ToArray();

            var standaloneFields = allFields
                .Where(x => x.GetCustomAttribute<SerializedPropertyGroupAttribute>() == null)
                .OrderBy(x => x.GetCustomAttribute<SerializedPropertyRepresentationAttribute>().Order)
                .ToArray();

            BuildFields(standaloneFields);

            var groupedFields = allFields
                .Where(x => x.GetCustomAttribute<SerializedPropertyGroupAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<SerializedPropertyGroupAttribute>().GetType().FullName)
                .ThenBy(x => x.GetCustomAttribute<SerializedPropertyGroupAttribute>().Order)
                .ThenBy(x => x.GetCustomAttribute<SerializedPropertyGroupAttribute>().Title)
                .GroupBy(x => (x.GetCustomAttribute<SerializedPropertyGroupAttribute>().GetType(), x.GetCustomAttribute<SerializedPropertyGroupAttribute>().Title))
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var key in groupedFields.Keys)
            {
                var title = groupedFields[key][0].GetCustomAttribute<SerializedPropertyGroupAttribute>().Title;

                if (key.Item1 == typeof(SerializedPropertyLabeledGroupAttribute))
                {
                    var fields = groupedFields[key];
                    LabeledArea(title, fields[0].GetCustomAttribute<SerializedPropertyLabeledGroupAttribute>().UseIntent, () => BuildFields(fields.ToArray()));
                }
                else if (key.Item1 == typeof(SerializedPropertyFoldingGroupAttribute))
                {
                    var fields = groupedFields[key];
                    FoldoutArea(title, fields[0].GetCustomAttribute<SerializedPropertyFoldingGroupAttribute>().UseIntent, () => BuildFields(fields.ToArray()));
                }
                else if (key.Item1 == typeof(SerializedPropertyTabGroupAttribute))
                {
                    var fields = groupedFields[key];
                    var fieldDict = fields
                        .OrderBy(x => x.GetCustomAttribute<SerializedPropertyTabGroupAttribute>().Order)
                        .ThenBy(x => x.GetCustomAttribute<SerializedPropertyTabGroupAttribute>().Name)
                        .GroupBy(x => x.GetCustomAttribute<SerializedPropertyTabGroupAttribute>().Name)
                        .ToDictionary(x => x.Key, x => x.ToList());
                    TabArea(title, fieldDict.Select(x => new TabItem(x.Key, () => BuildFields(x.Value.ToArray()))).ToArray());
                }
            }

            if (_customGuiPosition == CustomGUIPosition.Bottom)
            {
                DoInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void BuildFields(FieldInfo[] fields)
        {
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<SerializedPropertyRepresentationAttribute>();
                if (attribute == null)
                    continue;

                if (attribute.PreSpace > 0f)
                {
                    EditorGUILayout.Space(attribute.PreSpace);
                }

                if (attribute is SerializedPropertyDefaultRepresentationAttribute defaultAttribute)
                {
                    if (string.IsNullOrEmpty(defaultAttribute.Title))
                    {
                        EditorGUILayout.PropertyField((SerializedProperty) field.GetValue(this));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField((SerializedProperty) field.GetValue(this), new GUIContent(defaultAttribute.Title));
                    }
                }
                else if (attribute is SerializedPropertyImplicitRepresentationAttribute implicitAttribute)
                {
                    EditorGUILayout.PropertyField((SerializedProperty) field.GetValue(this), GUIContent.none, true);
                    ((SerializedProperty) field.GetValue(this)).isExpanded = true;
                }
                else if (attribute is SerializedPropertyIdentifiedArrayRepresentationAttribute arrayAttribute)
                {
                    ArrayArea(arrayAttribute.Title, (SerializedProperty[]) field.GetValue(this),
                        Enum.GetValues(arrayAttribute.EnumType).Cast<object>().ToArray(),
                        (o, property) => property.FindPropertyRelative("identifier").intValue == (int) o,
                        (o, property) => o.ToString());
                }
                else
                    throw new NotImplementedException();

                if (attribute.PostSpace > 0f)
                {
                    EditorGUILayout.Space(attribute.PostSpace);
                }
            }
        }

        #endregion

        protected virtual void DoInspectorGUI()
        {
        }

        public enum CustomGUIPosition : byte
        {
            Top,
            Bottom
        }
    }
}