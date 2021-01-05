using System.Collections.Generic;
using System.Collections.ObjectModel;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Types;
using UnityEngine;

namespace PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions
{
    public static class InputActionReferenceExtensions
    {
        private static readonly IDictionary<InputActionReference, InputAction> _actionCache = new Dictionary<InputActionReference, InputAction>();
        
        public static InputAction ToInputAction(this InputActionReference reference)
        {
            if (!_actionCache.ContainsKey(reference))
            {
                _actionCache.Add(reference, new InputAction());
            }

            return _actionCache[reference];
        }
    }
}