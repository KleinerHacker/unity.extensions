using System;
using PcSoft.ExtendedAnimation._90_Scripts.Types;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using PcSoft.ExtendedUI._90_Scripts.Utils.Extensions;
using PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Window
{
    [AddComponentMenu(ExtendedUIConstants.Menus.Components.Ui.WindowMenu + "/Dialog")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UiDialog : UIBehaviour
    {
        #region Inspector Data

        [SerializeField]
        private DialogState initialState = DialogState.Hidden;
        
        [SerializeField]
        private DialogEscapeAction escapeAction = DialogEscapeAction.None;

        [Header("SFX")]
        [SerializeField]
        private AudioMixerGroup audioMixerGroup;
        
        [SerializeField]
        private AudioClip openClip;
        
        [Header("Animation")]
        [SerializeField]
        private AnimationCurve fadingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        private float fadingSpeed = 1f;
        
        [SerializeField]
        private bool blockingGame = false;

        [SerializeField]
        private bool changeCursorSystem = false;

        #endregion

        #region Properties

        public DialogState State => _canvasGroup.IsShown() ? DialogState.Shown : DialogState.Hidden;

        public DialogEscapeAction EscapeAction => escapeAction;

        #endregion

        private AudioSource _audioSource;
        private CanvasGroup _canvasGroup;

        #region Builtin Methods

        protected override void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.outputAudioMixerGroup = audioMixerGroup;

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
                case DialogState.Hidden:
                    canvasGroup.Hide();
                    break;
                case DialogState.Shown:
                    canvasGroup.Show();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
#endif

        private void Update()
        {
            if (Input.GetButtonUp("Cancel"))
            {
                switch (EscapeAction)
                {
                    case DialogEscapeAction.None:
                        break;
                    case DialogEscapeAction.Toggle:
                        Debug.Log("Toggle dialog (escape)", this);
                        if (State == DialogState.Shown)
                            Hide();
                        else 
                            Show();
                        break;
                    case DialogEscapeAction.HideOnly:
                        if (State == DialogState.Shown)
                        {
                            Debug.Log("Hide dialog (escape)", this);
                            Hide();
                        }

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        public void Show()
        {
            if (State == DialogState.Shown)
            {
                Debug.LogWarning("Dialog already shown", this);
                return;
            }
            
            Debug.Log("Show dialog", this);

            StopAllCoroutines();

            if (openClip != null)
            {
                _audioSource.PlayOneShot(openClip);
            }

            _canvasGroup.Hide();
            StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed, 
                v => _canvasGroup.alpha = v, () =>
                {
                    _canvasGroup.Show();
                    if (changeCursorSystem)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                }));
            if (blockingGame)
            {
                GameTimeController.Pause(this, fadingCurve, fadingSpeed);
            }
        }

        public void Hide()
        {
            if (State == DialogState.Hidden)
            {
                Debug.LogWarning("Dialog already hidden", this);
                return;
            }
            
            Debug.Log("Hide dialog", this);

            StopAllCoroutines();
            
            if (changeCursorSystem)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            _canvasGroup.Hide();
            _canvasGroup.alpha = 1f;
            StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed, 
                v => _canvasGroup.alpha = 1f - v));
            if (blockingGame)
            {
                GameTimeController.Resume(this, fadingCurve, fadingSpeed);
            }
        }
    }

    public enum DialogState
    {
        Shown,
        Hidden,
    }
    
    public enum DialogEscapeAction
    {
        Toggle,
        HideOnly,
        None
    }
}