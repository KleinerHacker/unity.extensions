using System;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using UnityEngine.InputSystem.Controls;

namespace PcSoft.UnityInput._90_Scripts._90_Editor.Utils.Extensions
{
    internal static class InputValueExtensions
    {
        public static Type GetFitType(this InputValue value)
        {
            return value switch
            {
                InputValue.Button => typeof(ButtonControl),
                InputValue.Float => typeof(AxisControl),
                InputValue.Integer => typeof(IntegerControl),
                InputValue.Vector2 => typeof(Vector2Control),
                _ => throw new NotImplementedException()
            };
        }
    } 
}