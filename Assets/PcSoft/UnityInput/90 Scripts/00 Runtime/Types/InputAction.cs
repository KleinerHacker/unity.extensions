using System;
using System.Collections.Generic;
using UnityEngine;

namespace PcSoft.UnityInput._90_Scripts._00_Runtime.Types
{
    public sealed class InputAction
    {
        #region Events

        public event Action<InputActionContext> Performed;

        #endregion

        internal void RaisePerform(InputActionContext context)
        {
            Performed?.Invoke(context);
        }
    }

    public sealed class InputActionContext
    {
        private readonly IDictionary<string, object> _values;
        
        public InputAction InputAction { get; }

        public InputActionContext(InputAction inputAction, IDictionary<string, object> values)
        {
            _values = values;
            InputAction = inputAction;
        }

        public T ReadValue<T>(string key)
        {
            object value;
            try
            {
                value = _values[key];
            }
            catch (Exception)
            {
                throw new ArgumentException("Unable to find key " + key + " in values!");
            }
            
            try
            {
                return (T) value;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException("Unable to cast value of type " + value.GetType().FullName + " to type " + typeof(T).FullName);
            }
        }
    }
}