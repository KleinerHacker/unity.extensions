using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;

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
    }

    [Serializable]
    public sealed class InputItem
    {
        #region Inspector Data

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

        public InputType Type => type;

        public InputValue Value => value;

        public InputBehavior Behavior => behavior;

        public string Field => field;

        public InputActionReference[] Actions => actions;

        public InputItem[] SubItems => subItems;

        #endregion
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