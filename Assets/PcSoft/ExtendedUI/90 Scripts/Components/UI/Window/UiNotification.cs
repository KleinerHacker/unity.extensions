using PcSoft.ExtendedAnimation._90_Scripts.Types;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using UnityEngine;

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Window
{
    [AddComponentMenu(ExtendedUIConstants.Menus.Components.Ui.WindowMenu + "/Notification")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UiNotification : UiPopup
    {
        #region Inspector Data

        [Header("Behavior")]
        [SerializeField]
        [Range(0.5f, 10f)]
        private float showTime = 1f;

        #endregion

        #region Builtin Methods

        protected override void Awake()
        {
            base.Awake();

            if (initialState == ViewableState.Shown)
            {
                AutoHide();
            }
        }

        #endregion

        protected override void OnShown()
        {
            AutoHide();
        }

        private void AutoHide()
        {
            StartCoroutine(AnimationUtils.WaitAndRun(AnimationType.Unscaled, showTime, Hide));
        }
    }
}