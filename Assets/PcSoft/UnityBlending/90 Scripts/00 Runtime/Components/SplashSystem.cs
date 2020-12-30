using System;
using PcSoft.ExtendedAnimation._90_Scripts._00_Runtime.Types;
using PcSoft.ExtendedAnimation._90_Scripts._00_Runtime.Utils;
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
            OnShow(() => StartCoroutine(AnimationUtils.WaitAndRun(AnimationType.Unscaled, showTime, OnClose)));
        }

        #endregion

        protected abstract void OnShow(Action onFinished);
        protected abstract void OnClose();
    }
}