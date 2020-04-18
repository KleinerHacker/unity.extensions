using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Component
{
    [AddComponentMenu(ExtendedUIConstants.Menus.Components.Ui.ComponentMenu + "/Progress Bar")]
    [DisallowMultipleComponent]
    public sealed class UiProgressBar : UIBehaviour
    {
        #region Inspector Data
        
        [Header("Values")]
        [SerializeField]
        [Range(0f, 1f)]
        private float value = 0.5f;

        [Header("References")]
        [SerializeField]
        private Image progressImage;
        
        [SerializeField]
        private Image fillPointImage;

        [SerializeField]
        private float relativeX;

        #endregion

        public float Value
        {
            get => progressImage.fillAmount;
            set
            {
                progressImage.fillAmount = value;
                if (fillPointImage != null)
                {
                    var ownRectTrans = (RectTransform) transform;
                    var pointRectTrans = (RectTransform) fillPointImage.transform;
                    pointRectTrans.localPosition = new Vector3(relativeX + ownRectTrans.rect.width * value, pointRectTrans.localPosition.y, pointRectTrans.localPosition.z);
                }
            }
        }

        #region Builtin Methods
        
        protected override void Awake()
        {
            Value = value;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            Value = value;
        }  
#endif

        #endregion
    }
}