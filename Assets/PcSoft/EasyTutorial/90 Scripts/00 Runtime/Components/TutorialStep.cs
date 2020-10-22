using System;
using PcSoft.ExtendedAnimation._90_Scripts.Types;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PcSoft.EasyTutorial._90_Scripts._00_Runtime.Components
{
    [AddComponentMenu(EasyTutorialConstants.Menu.ComponentMenu + "/Tutorial Step")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class TutorialStep : MonoBehaviour
    {
        #region Inspector Data

        [SerializeField]
        private AnimationCurve fadingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        private float fadingSpeed = 0.5f;

        [SerializeField]
        private bool allowFading = true;

        [SerializeField]
        private Toggle ckbSkip;

        [Space]
        [Header("Events")]
        [SerializeField]
        private UnityEvent onShowStep;
        
        [SerializeField]
        private UnityEvent onHideStep;

        #endregion

        #region Events

        public event EventHandler Skip;

        #endregion
        
        private CanvasGroup _canvasGroup;

        #region Builtin Methods

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        #endregion

        public void Show()
        {
            if (allowFading)
            {
                StopAllCoroutines();
                StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed, v => _canvasGroup.alpha = v,
                    () => _canvasGroup.interactable = _canvasGroup.blocksRaycasts = true));
            }

            onShowStep.Invoke();
        }

        public void Hide()
        {
            if (ckbSkip != null && ckbSkip.isOn)
            {
                Skip?.Invoke(this, EventArgs.Empty);
            }

            if (allowFading)
            {
                StopAllCoroutines();
                StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed, v => _canvasGroup.alpha = 1f - v,
                    () => _canvasGroup.interactable = _canvasGroup.blocksRaycasts = false));
            }

            onHideStep.Invoke();
        }
    }
}