using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Types;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;
using InputAction = UnityEngine.InputSystem.InputAction;
using InputValue = PcSoft.UnityInput._90_Scripts._00_Runtime.Assets.InputValue;

namespace PcSoft.UnityInput._90_Scripts._00_Runtime.Components
{
    [AddComponentMenu(UnityInputConstants.Root + "/Input System")]
    [DisallowMultipleComponent]
    public sealed class InputSystem : MonoBehaviour
    {
        #region Inpsector Data

        [SerializeField]
        private InputPreset preset;

        #endregion

        private readonly IList<RuntimeControl> _runtimeControlList = new List<RuntimeControl>();

        #region Builtin Methods

        private void Awake()
        {
            foreach (var item in preset.Items)
            {
                HandleItem(item);
            }
        }

        private void Update()
        {
            foreach (var runtimeControl in _runtimeControlList)
            {
                runtimeControl.Run();
            }
        }

        #endregion

        private void HandleItem(InputItem item)
        {
            InputDevice inputDevice = item.Type switch
            {
                InputType.Mouse => Mouse.current,
                InputType.Keyboard => Keyboard.current,
                InputType.Pointer => Pointer.current,
                InputType.Touchscreen => Touchscreen.current,
                InputType.Gamepad => Gamepad.current,
                _ => throw new NotImplementedException()
            };
            if (inputDevice.deviceId == InputDevice.InvalidDeviceId)
                return;

            var inputElementPI = inputDevice.GetType().GetProperty(item.Field);
            var inputElement = (InputControl) inputElementPI.GetValue(inputDevice);

            switch (item.Value)
            {
                case InputValue.Button:
                    var getters = new List<TryGetValue>();
                    if (item.Behavior.HasFlag(InputBehavior.Press))
                    {
                        getters.Add((ref object oldValue, out object value) =>
                        {
                            var success = ((ButtonControl) inputElement).wasPressedThisFrame;
                            value = success;

                            return success;
                        });
                    }

                    if (item.Behavior.HasFlag(InputBehavior.Release))
                    {
                        getters.Add((ref object oldValue, out object value) =>
                        {
                            var success = ((ButtonControl) inputElement).wasReleasedThisFrame;
                            value = success;

                            return success;
                        });
                    }

                    if (item.Behavior.HasFlag(InputBehavior.Hold))
                    {
                        getters.Add((ref object oldValue, out object value) =>
                        {
                            var success = ((ButtonControl) inputElement).isPressed;
                            value = success;

                            return success;
                        });
                    }

                    _runtimeControlList.Add(new RuntimeControl(getters.ToArray(), false, item.Actions.Select(x => x.ToInputAction()).ToArray()));

                    break;
                case InputValue.Float:
                    _runtimeControlList.Add(new RuntimeControl((ref object oldValue, out object value) =>
                    {
                        value = ((AxisControl) inputElement).ReadValue();
                        var success = !Equals(value, oldValue);
                        oldValue = value;

                        return success;
                    }, 0f, item.Actions.Select(x => x.ToInputAction()).ToArray()));
                    break;
                case InputValue.Integer:
                    _runtimeControlList.Add(new RuntimeControl((ref object oldValue, out object value) =>
                    {
                        value = ((IntegerControl) inputElement).ReadValue();
                        var success = !Equals(value, oldValue);
                        oldValue = value;

                        return success;
                    }, 0, item.Actions.Select(x => x.ToInputAction()).ToArray()));
                    break;
                case InputValue.Vector2:
                    _runtimeControlList.Add(new RuntimeControl((ref object oldValue, out object value) =>
                    {
                        value = ((Vector2Control) inputElement).ReadValue();
                        var success = !Equals(value, oldValue);
                        oldValue = value;

                        return success;
                    }, Vector2.zero, item.Actions.Select(x => x.ToInputAction()).ToArray()));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private sealed class RuntimeControl
        {
            private readonly TryGetValue[] _getters;
            private readonly PcSoft.UnityInput._90_Scripts._00_Runtime.Types.InputAction[] _inputActions;

            private readonly object[] _oldValues;

            public RuntimeControl(TryGetValue getter, object oldValue, PcSoft.UnityInput._90_Scripts._00_Runtime.Types.InputAction[] inputActions)
                : this(new[] {getter}, oldValue, inputActions)
            {
            }

            public RuntimeControl(TryGetValue[] getters, object oldValue, PcSoft.UnityInput._90_Scripts._00_Runtime.Types.InputAction[] inputActions)
            {
                if (getters.Length <= 0)
                    throw new ArgumentException("Minimum one getter must exists");

                _getters = getters;
                _oldValues = new object[getters.Length];
                for (var i = 0; i < _oldValues.Length; i++)
                {
                    _oldValues[i] = oldValue;
                }

                _inputActions = inputActions;
            }

            public void Run()
            {
                object value = null;
                var success = _getters.Where((getter, i) => getter(ref _oldValues[i], out value)).Any();
                if (success)
                {
                    foreach (var inputAction in _inputActions)
                    {
                        inputAction.RaisePerform(new InputActionContext(_getters.Length > 1 ? success : value));
                    }
                }
            }
        }

        private delegate bool TryGetValue(ref object oldValue, out object value);
    }
}