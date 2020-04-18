using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PcSoft.UI._90_Scripts.Components.UI.Component
{
    [AddComponentMenu(ExtendedUIConstants.Menus.Components.Ui.ComponentMenu + "/Progress Bar")]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public sealed class UiProgressBar : UIBehaviour
    {
        #region Inspector Data

        [Header("References")]
        [SerializeField]
        private Image progressImage;

        [Header("Values")]
        [SerializeField]
        private float value = 0.5f;

        #endregion

        public float Value
        {
            get => progressImage.fillAmount;
            set => progressImage.fillAmount = value;
        }

        #region Builtin Methods

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            progressImage.fillAmount = value;
        }
#else
        protected override void Awake()
        {
            progressImage.fillAmount = value;
        }
#endif

        #endregion
    }
}