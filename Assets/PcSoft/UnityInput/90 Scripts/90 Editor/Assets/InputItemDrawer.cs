using System;
using System.Linq;
using System.Reflection;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;
using PcSoft.UnityInput._90_Scripts._90_Editor.Utils.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using InputValue = PcSoft.UnityInput._90_Scripts._00_Runtime.Assets.InputValue;
using Pointer = System.Reflection.Pointer;

namespace PcSoft.UnityInput._90_Scripts._90_Editor.Assets
{
    [CustomPropertyDrawer(typeof(InputItem))]
    public sealed class InputItemDrawer : ExtendedEditor._90_Scripts._90_Editor.ExtendedDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var actionsProperty = property.FindPropertyRelative("actions");
            return lineHeight * 6f + Math.Max(1, actionsProperty.arraySize) * lineHeight + 4f * 3f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProperty = property.FindPropertyRelative("type");
            var valueProperty = property.FindPropertyRelative("value");
            var behaviorProperty = property.FindPropertyRelative("behavior");
            var fieldProperty = property.FindPropertyRelative("field");
            var actionsProperty = property.FindPropertyRelative("actions");

            var rect = new Rect(position.x, position.y + 3f, position.width, lineHeight);
            EditorGUI.PropertyField(rect, typeProperty, new GUIContent("Type"));
            rect = CalculateNext(rect);
            EditorGUI.PropertyField(rect, valueProperty, new GUIContent("Value Type"));
            rect = CalculateNext(rect);
            if ((InputValue) valueProperty.intValue == InputValue.Button)
            {
                EditorGUI.PropertyField(rect, behaviorProperty, new GUIContent("Behavior"));
                rect = CalculateNext(rect);
            }

            (string name, string displayName)[] fields = ExtractFields((InputType) typeProperty.intValue, (InputValue) valueProperty.intValue);
            var curField = fieldProperty.stringValue;
            var curFieldIndex = fields.Select(x => x.name).ToList().IndexOf(curField);
            var newFieldIndex = EditorGUI.Popup(rect, curFieldIndex, fields.Select(x => x.displayName).ToArray());
            var newField = newFieldIndex < 0 || newFieldIndex >= fields.Length ? null : fields[newFieldIndex].name;
            fieldProperty.stringValue = newField;
            rect = CalculateNext(rect);

            EditorGUI.PropertyField(rect, actionsProperty, new GUIContent("Actions"));
            rect = CalculateNext(rect);
        }

        private static (string, string)[] ExtractFields(InputType type, InputValue value)
        {
            var device = type.GetFitDevice();
            return device.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => value.GetFitType().IsAssignableFrom(x.PropertyType))
                .Where(x => x.GetMethod.GetParameters().Length == 0)
                .Select(x =>
                {
                    try
                    {
                        return (x.Name, ((InputControl) x.GetValue(device)).displayName);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(x.Name + ": " + x.PropertyType.FullName + ": " + x.GetMethod + ": " + x.DeclaringType);
                        throw;
                    }
                })
                .OrderBy(x => x.displayName)
                .ToArray();
        }
    }
}