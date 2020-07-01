using System;
using PcSoft.ExtendedAnimation._90_Scripts.Types;
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

        private bool _cursorVisibility;
        private CursorLockMode _cursorLockMode;

        #region Builtin Methods

        protected override void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (EscapeAction != DialogEscapeAction.None && Input.GetKeyDown(KeyCode.F1))
#else
            if (EscapeAction != DialogEscapeAction.None && Input.GetButtonDown("Cancel"))
#endif
            {
                Debug.Log("Escape action");

                switch (EscapeAction)
                {
                    case DialogEscapeAction.None:
                        throw new NotSupportedException();
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
                    canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
                    break;
                case DialogState.Shown:
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
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
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
            StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed,
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
                        _cursorVisibility = Cursor.visible;
                        _cursorLockMode = Cursor.lockState;

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
                Cursor.visible = _cursorVisibility;
                Cursor.lockState = _cursorLockMode;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
            StartCoroutine(AnimationUtils.RunAnimation(AnimationType.Unscaled, fadingCurve, fadingSpeed,
                v =>
                {
                    canvasGroup.alpha = 1f - v;
                    if (blockingGame)
                    {
                        Time.timeScale = v;
                    }
                }, () => canvasGroup.interactable = canvasGroup.blocksRaycasts = true));
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