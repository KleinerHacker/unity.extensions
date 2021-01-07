using System;
using System.Collections.Generic;
using PcSoft.UnityInput._90_Scripts._00_Runtime;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Types;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;
using UnityEngine;

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

        private readonly IDictionary<InputAction, string> _dictionary = new Dictionary<InputAction, string>();

        #region Builtin Methods

        private void Awake()
        {
            foreach (var item in items)
            {
                _dictionary.Add(item.InputActionReference.ToInputAction(), item.Output);
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
            
            Debug.Log(_dictionary[obj.InputAction]);
        }
    }

    [Serializable]
    public sealed class InputDemoItem
    {
        #region Inspector Data

        [SerializeField]
        private InputActionReference inputActionReference;

        [SerializeField]
        private string output;

        #endregion

        #region Properties

        public InputActionReference InputActionReference => inputActionReference;

        public string Output => output;

        #endregion
    }
}
