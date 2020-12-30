using System;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Utils;
using PcSoft.SavePrefs._90_Scripts._00_Runtime.Utils;
using UnityEngine;

namespace PcSoft.EasyTutorial._90_Scripts._00_Runtime.Components
{
    public abstract class TutorialSystem<TS, T> : MonoBehaviour where TS : TutorialStepItem<T> where T : Enum
    {
        #region Inspector Data

        [SerializeField]
        protected TS[] steps;

        [SerializeField]
        protected string playerPrefKey;

        #endregion

        protected bool _active;
        protected readonly T _noneValue;

        protected TutorialSystem(T noneValue)
        {
            steps = ArrayUtils.CreateIdentifierArray<TS, T>(noneValue);
            _noneValue = noneValue;
        }

        #region Builtin Methods

        protected virtual void OnEnable()
        {
            _active = !IsFinished();

            foreach (var step in steps)
            {
                step.Step.Skip += StepOnSkip;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (var step in steps)
            {
                step.Step.Skip -= StepOnSkip;
            }
        }

        #endregion

        public void SendEvent(params T[] keys)
        {
            if (!_active)
                return;

            Debug.Log("Receive tutorial keys: " + string.Join(",", keys));
            HandleEvent(keys);
        }

        public void SkipTutorial()
        {
            if (!_active)
                return;

            Debug.Log("Skip Tutorial");

            MarkAsFinished();
            _active = false;

            OnSkipTutorial();
        }

        public virtual void ResetTutorial()
        {
            Debug.Log("Reset tutorial");

            PlayerPrefsEx.SetBool(playerPrefKey, false);
            _active = true;
        }

        private void StepOnSkip(object sender, EventArgs e)
        {
            SkipTutorial();
        }

        protected bool IsFinished()
        {
            return PlayerPrefsEx.GetBool(playerPrefKey, false);
        }

        protected void MarkAsFinished()
        {
            PlayerPrefsEx.SetBool(playerPrefKey, true, true);
        }

        protected virtual void OnSkipTutorial()
        {
        }

        protected abstract void HandleEvent(T[] keys);
    }

    [Serializable]
    public abstract class TutorialStepItem<T> : IIdentifiedObject<T> where T : Enum
    {
        #region Inspector Data

        [HideInInspector]
        [SerializeField]
        private T identifier;

        [SerializeField]
        private TutorialStep step;

        #endregion

        #region Properties

        public T Identifier => identifier;

        public TutorialStep Step => step;

        #endregion

        protected TutorialStepItem(T identifier)
        {
            this.identifier = identifier;
        }
    }
}