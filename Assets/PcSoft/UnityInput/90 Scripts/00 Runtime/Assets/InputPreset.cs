using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

namespace PcSoft.UnityInput._90_Scripts._00_Runtime.Assets
{
    [CreateAssetMenu(menuName = UnityInputConstants.Root + "/Input Preset", fileName = "InputPreset")]
    public sealed class InputPreset : ScriptableObject
    {
        #region Inspector Data

        [SerializeField]
        private InputItem[] items;

        #endregion

        #region Properties

        public InputItem[] Items => items;

        #endregion

        #region Builtin Methods

        private void OnValidate()
        {
            ValidateNextItems(items);
        }

        #endregion

        private void ValidateNextItems(InputItem[] items)
        {
            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    item.Name = Guid.NewGuid().ToString();
                }
                
                ValidateNextItems(item.SubItems);
            }
        }
    }

    [Serializable]
    public sealed class InputItem
    {
        #region Inspector Data

        [SerializeField]
        private string name;

        [SerializeField]
        private InputType type;

        [SerializeField]
        private InputValue value;

        [SerializeField]
        private InputBehavior behavior = InputBehavior.Press;

        [SerializeField]
        private string field;

        [SerializeField]
        private InputActionReference[] actions;

        [SerializeField]
        private InputItem[] subItems;

        #endregion

        #region Properties

        public string Name
        {
            get => name;
            internal set => name = value;
        }

        public InputType Type => type;

        public InputValue Value => value;

        public InputBehavior Behavior => behavior;

        public string Field => field;

        public InputActionReference[] Actions => actions;

        public InputItem[] SubItems => subItems;

        #endregion

        public override string ToString()
        {
            return $"{type} / {value} {(value == InputValue.Button ? " / " + behavior : "")} {(string.IsNullOrWhiteSpace(name) ? "" : " / " + name)}";
        }
    }

    public enum InputType
    {
        Mouse,
        Keyboard,
        Pointer,
        Touchscreen,
        Gamepad,
    }

    public enum InputValue
    {
        Button,
        Float,
        Integer,
        Vector2,
    }

    public enum InputBehavior
    {
        Press,
        Release,
        Hold,
    }
}