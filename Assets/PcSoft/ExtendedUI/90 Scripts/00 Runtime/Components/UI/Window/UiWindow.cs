using System;
using PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Utils;
using UnityEngine;
using UnityEngine.Audio;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Assets;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Types;
using PcSoft.UnityInput._90_Scripts._00_Runtime.Utils.Extensions;

namespace PcSoft.ExtendedUI._90_Scripts._00_Runtime.Components.UI.Window
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
        [SerializeField]
        private InputActionReference toggler;

        #endregion

        #region Properties

        public DialogEscapeAction EscapeAction => escapeAction;

        #endregion

        private AudioSource _audioSource;
        private InputAction _togglerAction;

        #region Builtin Methods

        protected override void Awake()
        {
            base.Awake();

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            _audioSource.outputAudioMixerGroup = audioMixerGroup;

            _togglerAction = toggler.ToInputAction();
        }

        protected override void OnEnable()
        {
            _togglerAction.Performed += PlayerInputOnActionTriggered;
        }

        protected override void OnDisable()
        {
            _togglerAction.Performed -= PlayerInputOnActionTriggered;
        }

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

        private void PlayerInputOnActionTriggered(InputActionContext obj)
        {
            HandleToggle();
        }

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