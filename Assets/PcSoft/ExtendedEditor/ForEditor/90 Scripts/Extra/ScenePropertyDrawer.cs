using PcSoft.ExtendedEditor._90_Scripts.Editor;
using PcSoft.ExtendedEditor.ForGame._90_Scripts.Extra;
using UnityEditor;
using UnityEngine;

namespace PcSoft.ExtendedEditor.ForEditor._90_Scripts.Extra
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public sealed class ScenePropertyDrawer : ExtendedDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
            
            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUILayout.ObjectField(label, oldScene, typeof(SceneAsset), false) as SceneAsset;
            
            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                property.stringValue = newPath;
            }
        }
    }
}