using System;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions;
using UnityEditor;

namespace PcSoft.EasyTutorial._90_Scripts._90_Editor.Components
{
    public abstract class TutorialSystemEditor<T> : ExtendedEditor._90_Scripts._90_Editor.ExtendedEditor where T : Enum
    {
        private readonly T _noneValue;
        
        private SerializedProperty _playerPrefKeyProperty;
        private SerializedProperty[] _stepsProperties;

        protected TutorialSystemEditor(T noneValue)
        {
            _noneValue = noneValue;
        }

        protected virtual void OnEnable()
        {
            _playerPrefKeyProperty = serializedObject.FindProperty("playerPrefKey");
            _stepsProperties = serializedObject.FindProperties("steps");
        }

        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_playerPrefKeyProperty);
            OnInspectorGUIAfterPrefKey();
            EditorGUILayout.Space();
            ArrayArea("Steps", _stepsProperties, Enum.GetValues(typeof(T)).Cast<T>().Where(x => !Equals(x, _noneValue)).ToArray(),
                (e, p) => Equals((T) Enum.ToObject(typeof(T), p.FindPropertyRelative("identifier").intValue), e),
                (e, p) => e.ToString());
            OnInspectorGUIAfterSteps();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnInspectorGUIAfterPrefKey()
        {
        }

        protected virtual void OnInspectorGUIAfterSteps()
        {
        }
    }
}