using System;
using System.Collections.Generic;
using System.Text;
using PcSoft.UnityInput._90_Scripts._00_Runtime;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Types;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace PcSoft.UnityInput._90_Scripts._80_Demo.Components
{
    [AddComponentMenu(UnityInputConstants.Root + "/Demo Handler")]
    [DisallowMultipleComponent]
    public class DemoHandler : MonoBehaviour
    {
        #region Inspector Data

        [SerializeField]
        private InputDemoItem[] items;

        #endregion

        private readonly IDictionary<InputAction, InputDemoItem> _dictionary = new Dictionary<InputAction, InputDemoItem>();

        #region Builtin Methods

        private void Awake()
        {
            foreach (var item in items)
            {
                _dictionary.Add(item.InputActionReference.ToInputAction(), item);
            }
        }

        private void OnEnable()
        {
            foreach (var inputAction in _dictionary.Keys)
            {
                inputAction.Performed += InputActionOnPerformed;
            }
        }

        private void OnDisable()
        {
            foreach (var inputAction in _dictionary.Keys)
            {
                inputAction.Performed -= InputActionOnPerformed;
            }
        }

        #endregion

        private void InputActionOnPerformed(InputActionContext obj)
        {
            if (!_dictionary.ContainsKey(obj.InputAction))
                return;

            var demoItem = _dictionary[obj.InputAction];
            var sb = new StringBuilder();
            sb.Append(demoItem.Output);

            foreach (var output in demoItem.AdditionalOutputs)
            {
                sb.Append(" > ").Append(output.Key).Append(": ");
                if (output.ShowBoolean)
                {
                    sb.Append(obj.ReadValue<bool>(output.Key));
                }
                else if (output.ShowFloat)
                {
                    sb.Append(obj.ReadValue<float>(output.Key));
                }
                else if (output.ShowInteger)
                {
                    sb.Append(obj.ReadValue<int>(output.Key));
                }
                else if (output.ShowVector2)
                {
                    sb.Append(obj.ReadValue<Vector2>(output.Key));
                }
                else
                {
                    sb.Append("Unknown");
                }

                sb.Append(" < ");
            }

            Debug.Log(sb.ToString());
        }
    }

    [Serializable]
    public sealed class InputDemoItem
    {
        #region Inspector Data

        [SerializeField]
        private InputActionReference inputActionReference;

        [Header("Output")]
        [SerializeField]
        private string output;

        [SerializeField]
        private InputDemoOutput[] additionalOutputs;

        #endregion

        #region Properties

        public InputActionReference InputActionReference => inputActionReference;

        public string Output => output;

        public InputDemoOutput[] AdditionalOutputs => additionalOutputs;

        #endregion
    }

    [Serializable]
    public sealed class InputDemoOutput
    {
        #region Inspector Data

        [SerializeField]
        private string key;

        [Space]
        [SerializeField]
        private bool showBoolean;

        [SerializeField]
        private bool showFloat;

        [SerializeField]
        private bool showInteger;

        [SerializeField]
        private bool showVector2;

        #endregion

        #region Properties

        public string Key => key;

        public bool ShowBoolean => showBoolean;

        public bool ShowFloat => showFloat;

        public bool ShowInteger => showInteger;

        public bool ShowVector2 => showVector2;

        #endregion
    }
}