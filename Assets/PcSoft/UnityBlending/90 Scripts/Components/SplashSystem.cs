using System;
using PcSoft.ExtendedAnimation._90_Scripts.Types;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using UnityEngine;

namespace PcSoft.UnityBlending._90_Scripts.Components
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
            OnShow(() => StartCoroutine(AnimationUtils.WaitAndRun(AnimationType.Unscaled, showTime, () => OnClose(onFinished: GotoNextScene))));
        }

        #endregion

        protected abstract void OnShow(Action onFinished);
        protected abstract void OnClose(Action onFinished);
        protected abstract void GotoNextScene();
    }
}