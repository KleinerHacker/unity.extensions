using System;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Utils;
using PcSoft.SavePrefs._90_Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PcSoft.EasyTutorial._90_Scripts._00_Runtime.Components
{
    public abstract class SequentialTutorialSystem<TS, T> : TutorialSystem<TS, T> where TS : SequentialTutorialStepItem<T> where T : Enum
    {
        #region Inspector Data

        [SerializeField]
        private TutorialStep lastStep;

        [SerializeField]
        private Button btnCurrent;

        [SerializeField]
        private Text txtCurrent;

        [SerializeField]
        private GameObject current;

        [SerializeField]
        private float autoStartDelay = 1f;

        #endregion

        #region Properties

        public byte CurrentStep => _stepIndex;

        #endregion

        private byte _stepIndex = 0;
        private T _currentKey;

        protected SequentialTutorialSystem(T noneValue) : base(noneValue)
        {
        }

        #region Builtin Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (_active)
            {
                StartCoroutine(AnimationUtils.WaitAndRun(autoStartDelay, () =>
                {
                    current.SetActive(true);
                    ShowNextStep(true);
                }));
            }
        }

        #endregion

        public void ShowCurrentStep()
        {
            if (_stepIndex >= steps.Length)
                return;
            
            steps[_stepIndex].Step.Show();
            txtCurrent.text = steps[_stepIndex].StepInfo;
        }

        protected override void HandleEvent(T[] keys)
        {
            if (IsHandleEvent(_currentKey, keys, out var acceptKey))
            {
                _currentKey = acceptKey;
                ShowNextStep();
            }
        }

        protected abstract bool IsHandleEvent(T currentKey, T[] eventKeys, out T acceptKey);

        private void ShowNextStep(bool first = false)
        {
            if (!first)
            {
                _stepIndex++;
            }

            if (_stepIndex >= steps.Length)
            {
                MarkAsFinished();
                
                current.SetActive(false);
                ShowLastStep();

                _active = false;
                return;
            }
            
            ShowCurrentStep();
        }

        private void ShowLastStep()
        {
            lastStep.Show();
        }

        protected override void OnSkipTutorial()
        {
            current.SetActive(false);
        }
    }

    [Serializable]
    public abstract class SequentialTutorialStepItem<T> : TutorialStepItem<T> where T : Enum
    {
        #region Properties

        public abstract string StepInfo { get; }

        #endregion

        protected SequentialTutorialStepItem(T identifier) : base(identifier)
        {
        }
    }
}