using System;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
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

        [SerializeField]
        [HideInInspector]
        private CanvasGroup canvasGroup;

        #endregion

        #region Properties

        public DialogState State => gameObject.activeSelf ? DialogState.Shown : DialogState.Hidden;

        public DialogEscapeAction EscapeAction => escapeAction;

        #endregion

        private AudioSource _audioSource;

        #region Builtin Methods

        protected override void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (EditorApplication.isPlaying)
                return;
            
            canvasGroup = GetComponent<CanvasGroup>();
            
            switch (initialState)
            {
                case DialogState.Hidden:
                    canvasGroup.alpha = 0f;
                    gameObject.SetActive(false);
                    break;
                case DialogState.Shown:
                    canvasGroup.alpha = 1f;
                    gameObject.SetActive(true);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
#endif

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

            canvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            StartCoroutine(AnimationUtils.RunAnimationUnscaled(fadingCurve, fadingSpeed, 
                v =>
                {
                    canvasGroup.alpha = v;
                    if (blockingGame)
                    {
                        Time.timeScale = 1f - v;
                    }
                }, () =>
                {
                    if (changeCursorSystem)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                }));
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

            canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
            StartCoroutine(AnimationUtils.RunAnimationUnscaled(fadingCurve, fadingSpeed, 
                v =>
                {
                    canvasGroup.alpha = 1f - v;
                    if (blockingGame)
                    {
                        Time.timeScale = v;
                    }
                }, () => gameObject.SetActive(false)));
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