using System;
using PcSoft.ExtendedAnimation._90_Scripts.Types;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using PcSoft.ExtendedUI._90_Scripts.Utils.Extensions;
using PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Window
{
    public abstract class UiStage : UIBehaviour, IViewable
    {
        #region Inspector Data

        [Header("Animation")]
        [SerializeField]
        protected AnimationCurve fadingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        protected float fadingSpeed = 1f;
        
        [Header("Behavior")]
        [SerializeField]
        protected ViewableState initialState = ViewableState.Hidden;

        #endregion
        
        #region Properties

        public ViewableState State => _canvasGroup.IsShown() ? ViewableState.Shown : ViewableState.Hidden;

        #endregion
        
        private CanvasGroup _canvasGroup;

        #region Builtin Methods

        protected override void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (EditorApplication.isPlaying)
                return;
            
            var canvasGroup = GetComponent<CanvasGroup>();
            
            switch (initialState)
            {
                case ViewableState.Hidden:
                    canvasGroup.Hide();
                    break;
                case ViewableState.Shown:
                    canvasGroup.Show();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
#endif

        #endregion
        
        public void Show()
        {
            if (State == ViewableState.Shown)
            {
                Debug.LogWarning("Dialog already shown", this);
                return;
            }
            
            Debug.Log("Show dialog", this);

            StopAllCoroutines();
            OnShowing();

            _canvasGroup.Hide();
            StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed, 
                v => _canvasGroup.alpha = v, () =>
                {
                    _canvasGroup.Show();
                    OnShown();
                }));
        }

        public void Hide()
        {
            if (State == ViewableState.Hidden)
            {
                Debug.LogWarning("Dialog already hidden", this);
                return;
            }
            
            Debug.Log("Hide dialog", this);

            StopAllCoroutines();
            OnHiding();

            _canvasGroup.Hide();
            _canvasGroup.alpha = 1f;
            StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed, 
                v => _canvasGroup.alpha = 1f - v, OnHidden));
        }
        
        protected virtual void OnShowing() {}
        protected virtual void OnShown() {}
        
        protected virtual void OnHiding() {}
        protected virtual void OnHidden() {}
    }
}