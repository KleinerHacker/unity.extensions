using System;
using UnityAnimation.Runtime.animation.Scripts.Types;
using UnityAnimation.Runtime.animation.Scripts.Utils;
using UnityEngine;

namespace PcSoft.UnityBlending._90_Scripts._00_Runtime.Components
{
    public abstract class SplashSystem : MonoBehaviour
    {
        #region Inspector Data

        [SerializeField]
        private float showTime = 3f;

        #endregion

        #region Builtin Methods

        private void Start()
        {
            OnShow(() =>
                AnimationBuilder.Create(this, AnimationType.Unscaled)
                    .Wait(showTime, OnClose)
                    .Start()
            );
        }

        #endregion

        protected abstract void OnShow(Action onFinished);
        protected abstract void OnClose();
    }
}