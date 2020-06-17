using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor.Extra
{
    [CustomPropertyDrawer(typeof(LayerMaskAttribute))]
    public sealed class LayerMaskPropertyDrawer : ExtendedDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var mask = property.intValue;
            mask = EditorGUI.MaskField(position, label, mask, InternalEditorUtility.layers);
            property.intValue = mask;
        }
    }
}