using UnityEditor;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor.Utils.Extensions
{
    public static class EditorExtensions
    {
        public static SerializedProperty[] FindPropertiesRelative(this SerializedProperty property, string name)
        {
            var relativeSize = property.FindPropertyRelative(name + ".Array.size").intValue;
            var relativeProperties = new SerializedProperty[relativeSize];
            for (var i = 0; i < relativeSize; i++)
            {
                relativeProperties[i] = property.FindPropertyRelative(name + ".Array.data[" + i + "]");
            }

            return relativeProperties;
        }

        public static SerializedProperty[] FindProperties(this SerializedObject o, string name)
        {
            var size = o.FindProperty(name + ".Array.size").intValue;
            var properties = new SerializedProperty[size];
            for (var i = 0; i < size; i++)
            {
                properties[i] = o.FindProperty(name + ".Array.data[" + i + "]");
            }

            return properties;
        }
    }
}