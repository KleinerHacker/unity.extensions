using System;
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
        private object _value;

        public InputActionContext(object value)
        {
            _value = value;
        }

        public T ReadValue<T>()
        {
            try
            {
                return (T) _value;
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException("Unable to cast value of type " + _value.GetType().FullName + " to type " + typeof(T).FullName);
            }
        }
    }
}