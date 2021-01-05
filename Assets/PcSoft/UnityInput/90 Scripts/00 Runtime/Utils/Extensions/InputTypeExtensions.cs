using System;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using UnityEngine.InputSystem;

namespace PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions
{
    public static class InputTypeExtensions
    {
        public static InputDevice GetFitDevice(this InputType type)
        {
            return type switch
            {
                InputType.Keyboard => Keyboard.current,
                InputType.Mouse => Mouse.current,
                InputType.Pointer => Pointer.current,
                InputType.Touchscreen => Touchscreen.current,
                InputType.Gamepad => Gamepad.current,
                _ => throw new NotImplementedException()
            };
        }
    }
}