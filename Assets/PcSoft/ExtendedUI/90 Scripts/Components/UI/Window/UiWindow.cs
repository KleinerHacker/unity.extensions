using System;
using PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Utils;
using UnityEngine;
using UnityEngine.Audio;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PcSoft.ExtendedUI._90_Scripts.Components.UI.Window
{
    [AddComponentMenu(ExtendedUIConstants.Menus.Components.Ui.WindowMenu + "/Window")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class UiWindow : UiStage
    {
        #region Inspector Data

        [SerializeField]
        private DialogEscapeAction escapeAction = DialogEscapeAction.None;

        [Header("SFX")]
        [SerializeField]
        private AudioMixerGroup audioMixerGroup;

        [SerializeField]
        private AudioClip openClip;

        [Header("Game Behavior")]
        [SerializeField]
        private bool blockingGame = false;

        [SerializeField]
        private bool changeCursorSystem = false;

        [Header("Input System")]
#if ENABLE_INPUT_SYSTEM
        [SerializeField]
        private InputActionAsset inputAction;

        [SerializeField]
        private string actionId;
#else
        [SerializeField]
        private string action = "Cancel";
#endif

        #endregion

        #region Properties

        public DialogEscapeAction EscapeAction => escapeAction;

        #endregion

        private AudioSource _audioSource;
#if ENABLE_INPUT_SYSTEM
        private InputAction _ia;
#endif

        #region Builtin Methods

        protected override void Awake()
        {
            base.Awake();

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.outputAudioMixerGroup = audioMixerGroup;

#if ENABLE_INPUT_SYSTEM
            _ia = inputAction.FindAction(actionId, true);
#endif
        }

        protected override void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            _ia.performed += PlayerInputOnActionTriggered;
            _ia.Enable();
#endif
        }

        protected override void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            _ia.Disable();
            _ia.performed -= PlayerInputOnActionTriggered;
#endif
        }

#if !ENABLE_INPUT_SYSTEM
        private void Update()
        {
            if (Input.GetButtonUp(action))
            {
                HandleToggle();
            }
        }
#endif

#if ENABLE_INPUT_SYSTEM && UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (inputAction == null || !string.IsNullOrEmpty(actionId))
                return;

            var enumerator = inputAction.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                actionId = current.id.ToString();
            }
        }
#endif

        #endregion

        private void HandleToggle()
        {
            switch (EscapeAction)
            {
                case DialogEscapeAction.None:
                    break;
                case DialogEscapeAction.Toggle:
                    Debug.Log("Toggle dialog (escape)", this);
                    if (State == ViewableState.Shown)
                        Hide();
                    else
                        Show();
                    break;
                case DialogEscapeAction.HideOnly:
                    if (State == ViewableState.Shown)
                    {
                        Debug.Log("Hide dialog (escape)", this);
                        Hide();
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

#if ENABLE_INPUT_SYSTEM
        private void PlayerInputOnActionTriggered(InputAction.CallbackContext obj)
        {
            HandleToggle();
        }
#endif

        protected override void OnShowing()
        {
            if (openClip != null)
            {
                _audioSource.PlayOneShot(openClip);
            }

            if (blockingGame)
            {
                GameTimeController.Pause(this, fadingCurve, fadingSpeed);
            }
        }

        protected override void OnShown()
        {
            if (changeCursorSystem)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        protected override void OnHiding()
        {
            if (changeCursorSystem)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (blockingGame)
            {
                GameTimeController.Resume(this, fadingCurve, fadingSpeed);
            }
        }
    }

    public enum DialogEscapeAction
    {
        Toggle,
        HideOnly,
        None
    }
}