using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Types;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using InputAction = PcSoft.UnityInput._90_Scripts._00_Runtime.Types.InputAction;
using InputValue = PcSoft.UnityInput._90_Scripts._00_Runtime.Assets.InputValue;

// ReSharper disable HeapView.BoxingAllocation

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
            var runtimeControl = CreateControlFromItem(item);
            if (runtimeControl == null)
                return;
            
            _runtimeControlList.Add(runtimeControl);
        }

        private RuntimeControl CreateControlFromItem(InputItem item)
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
                return null;

            var inputElementPI = inputDevice.GetType().GetProperty(item.Field);
            var inputElement = (InputControl) inputElementPI.GetValue(inputDevice);


            (object defValue, TryGetValueDelegate getter) value = item.Value switch
            {
                InputValue.Button =>
                    item.Behavior switch
                    {
                        InputBehavior.Press => (false, (ref object oldValue, out object value) =>
                        {
                            var success = ((ButtonControl) inputElement).wasPressedThisFrame;
                            value = success;

                            return success;
                        }),
                        InputBehavior.Release => (false, (ref object oldValue, out object value) =>
                        {
                            var success = ((ButtonControl) inputElement).wasReleasedThisFrame;
                            value = success;

                            return success;
                        }),
                        InputBehavior.Hold => (false, (ref object oldValue, out object value) =>
                        {
                            var success = ((ButtonControl) inputElement).isPressed;
                            value = success;

                            return success;
                        }),
                        _ => throw new NotImplementedException(),
                    },
                InputValue.Float => (0f, (ref object oldValue, out object value) =>
                {
                    value = ((AxisControl) inputElement).ReadValue();
                    var success = !Equals(value, oldValue);
                    oldValue = value;

                    return success;
                }),
                InputValue.Integer => (0, (ref object oldValue, out object value) =>
                {
                    value = ((IntegerControl) inputElement).ReadValue();
                    var success = !Equals(value, oldValue);
                    oldValue = value;

                    return success;
                }),
                InputValue.Vector2 => (Vector2.zero, (ref object oldValue, out object value) =>
                {
                    value = ((Vector2Control) inputElement).ReadValue();
                    var success = !Equals(value, oldValue);
                    oldValue = value;

                    return success;
                }),
                _ => throw new NotImplementedException(),
            };

            var inputActions = item.Actions.Select(x => x.ToInputAction()).ToArray();
            var subControls = item.SubItems.Select(CreateControlFromItem).ToArray();
            
            return new RuntimeControl(value.getter, value.defValue, inputActions, subControls);
        }

        private sealed class RuntimeControl
        {
            private readonly TryGetValueDelegate _getter;
            private readonly InputAction[] _inputActions;
            private readonly RuntimeControl[] _subControls;

            private object _oldValue;

            public RuntimeControl(TryGetValueDelegate getter, object oldValue, InputAction[] inputActions, RuntimeControl[] subControls)
            {
                _getter = getter;
                _oldValue = oldValue;
                _inputActions = inputActions;
                _subControls = subControls;
            }

            public bool Run()
            {
                var success = _getter(ref _oldValue, out var value);
                if (success)
                {
                    if (_subControls.Any(x => x.Run()))
                        return true; //Break recursion if sub control hits (priority for sub controls)
                    
                    foreach (var inputAction in _inputActions)
                    {
                        inputAction.RaisePerform(new InputActionContext(value));
                    }

                    return true;
                }

                return false;
            }
        }

        private delegate bool TryGetValueDelegate(ref object oldValue, out object value);
    }
}