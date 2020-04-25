using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra;
using UnityEditor;
using UnityEngine;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor.Extra
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public sealed class ScenePropertyDrawer : ExtendedDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
            
            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUI.ObjectField(position, label, oldScene, typeof(SceneAsset), false) as SceneAsset;
            
            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                property.stringValue = newPath;
            }
        }
    }
}