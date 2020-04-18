using PcSoft.ExtendedEditor._90_Scripts.Editor;
using PcSoft.ExtendedEditor.ForGame._90_Scripts.Extra;
using UnityEditor;
using UnityEngine;

namespace PcSoft.ExtendedEditor.ForEditor._90_Scripts.Extra
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