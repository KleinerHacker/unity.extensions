using System;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions;
using UnityEditor;
using UnityEngine;

namespace PcSoft.EasyTutorial._90_Scripts._90_Editor.Components
{
    public abstract class SequentialTutorialSystemEditor<T> : TutorialSystemEditor<T> where T : Enum
    {
        private SerializedProperty _lastStepProperty;
        private SerializedProperty _btnCurrentProperty;
        private SerializedProperty _txtCurrentProperty;
        private SerializedProperty _currentProperty;
        private SerializedProperty _autoStartDelayProperty;

        protected SequentialTutorialSystemEditor(T noneValue) : base(noneValue)
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _lastStepProperty = serializedObject.FindProperty("lastStep");
            _btnCurrentProperty = serializedObject.FindProperty("btnCurrent");
            _txtCurrentProperty = serializedObject.FindProperty("txtCurrent");
            _currentProperty = serializedObject.FindProperty("current");
            _autoStartDelayProperty = serializedObject.FindProperty("autoStartDelay");
        }

        protected override void OnInspectorGUIAfterPrefKey()
        {
            EditorGUILayout.PropertyField(_autoStartDelayProperty);
        }

        protected override void OnInspectorGUIAfterSteps()
        {
            EditorGUILayout.PropertyField(_lastStepProperty);
            EditorGUILayout.Space();
            LabeledArea("References", () =>
            {
                EditorGUILayout.PropertyField(_currentProperty, new GUIContent("Current Activity GO"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_btnCurrentProperty, new GUIContent("Current Activity Button"));
                EditorGUILayout.PropertyField(_txtCurrentProperty, new GUIContent("Current Activity Text"));
            });
        }
    }
}