using System;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions;
using UnityEditor;
using UnityEngine;

namespace PcSoft.EasyTutorial._90_Scripts._90_Editor.Components
{
    public abstract class TutorialSystemEditor<T> : ExtendedEditor._90_Scripts._90_Editor.ExtendedEditor where T : Enum
    {
        private readonly T _noneValue;
        
        private SerializedProperty _playerPrefKeyProperty;
        private SerializedProperty[] _stepsProperties;
        private SerializedProperty _lastStepProperty;
        private SerializedProperty _btnCurrentProperty;
        private SerializedProperty _txtCurrentProperty;
        private SerializedProperty _currentProperty;
        private SerializedProperty _autoStartDelayProperty;

        protected TutorialSystemEditor(T noneValue)
        {
            _noneValue = noneValue;
        }

        private void OnEnable()
        {
            _playerPrefKeyProperty = serializedObject.FindProperty("playerPrefKey");
            _stepsProperties = serializedObject.FindProperties("steps");
            _lastStepProperty = serializedObject.FindProperty("lastStep");
            _btnCurrentProperty = serializedObject.FindProperty("btnCurrent");
            _txtCurrentProperty = serializedObject.FindProperty("txtCurrent");
            _currentProperty = serializedObject.FindProperty("current");
            _autoStartDelayProperty = serializedObject.FindProperty("autoStartDelay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_playerPrefKeyProperty);
            EditorGUILayout.PropertyField(_autoStartDelayProperty);
            EditorGUILayout.Space();
            ArrayArea("Steps", _stepsProperties, Enum.GetValues(typeof(T)).Cast<T>().Where(x => !Equals(x, _noneValue)).ToArray(),
                (e, p) => Equals((T) Enum.ToObject(typeof(T), p.FindPropertyRelative("identifier").intValue), e),
                (e, p) => e.ToString());
            EditorGUILayout.PropertyField(_lastStepProperty);
            EditorGUILayout.Space();
            LabeledArea("References", () =>
            {
                EditorGUILayout.PropertyField(_currentProperty, new GUIContent("Current Activity GO"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_btnCurrentProperty, new GUIContent("Current Activity Button"));
                EditorGUILayout.PropertyField(_txtCurrentProperty, new GUIContent("Current Activity Text"));
            });

            serializedObject.ApplyModifiedProperties();
        }
    }
}