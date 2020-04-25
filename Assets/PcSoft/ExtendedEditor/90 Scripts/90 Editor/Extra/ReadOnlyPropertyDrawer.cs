using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra;
using UnityEditor;
using UnityEngine;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor.Extra
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public sealed class ReadOnlyPropertyDrawer : ExtendedDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}