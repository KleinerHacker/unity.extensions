using System;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Components
{
    public abstract class SingletonBehavior<T> : MonoBehaviour where T : SingletonBehavior<T>
    {
        public static T Singleton { get; private set; }

        protected virtual void OnEnable()
        {
            Singleton = (T) this;
        }

        protected virtual void OnDisable()
        {
            Singleton = null;
        }
    }
}