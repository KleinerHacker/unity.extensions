﻿using System;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions;
using UnityEditor;
using UnityEngine;

namespace PcSoft.UnityScene._90_Scripts._90_Editor.Components
{
    public abstract class SceneSystemEditor<T> : ExtendedEditor._90_Scripts._90_Editor.ExtendedEditor where T : Enum
    {
        private SerializedProperty[] _sceneProperties;
        private SerializedProperty _initialStateProperty;
        private SerializedProperty _blendingProperty;
        private SerializedProperty _masterSceneProperty;

        protected virtual void OnEnable()
        {
            _sceneProperties = serializedObject.FindProperties("scenes");
            _initialStateProperty = serializedObject.FindProperty("initialState");
            _blendingProperty = serializedObject.FindProperty("blending");
            _masterSceneProperty = serializedObject.FindProperty("masterScene");
        }

        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_masterSceneProperty, new GUIContent("Master Scene"));
            ArrayArea("Scenes", _sceneProperties, Enum.GetValues(typeof(T)).Cast<T>().ToArray(),
                (e, p) => Equals((T) Enum.ToObject(typeof(T), p.FindPropertyRelative("identifier").intValue), e),
                (e, p) => e.ToString());
            EditorGUILayout.PropertyField(_initialStateProperty, new GUIContent("Initial State"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_blendingProperty, new GUIContent("Blending"));
            
            OnCustomGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnCustomGUI()
        {
        }
    }
}