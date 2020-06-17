using System;
using PcSoft.ExtendedAnimation._90_Scripts.Utils;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PcSoft.EasyTutorial._90_Scripts._00_Runtime.Components
{
    public abstract class TutorialSystem<TS, T> : MonoBehaviour where TS : TutorialStepItem<T> where T : Enum
    {
        #region Inspector Data

        [SerializeField]
        private TS[] steps;

        [SerializeField]
        private TutorialStep lastStep;

        [SerializeField]
        private string playerPrefKey;

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

        private bool _active;
        private byte _stepIndex = 0;
        private T _currentKey;

        protected TutorialSystem(T noneValue)
        {
            steps = ArrayUtils.CreateIdentifierArray<TS, T>(noneValue);
        }

        #region Builtin Methods

        protected virtual void OnEnable()
        {
            _active = PlayerPrefs.GetInt(playerPrefKey, 0) == 0;
            if (_active)
            {
                StartCoroutine(AnimationUtils.WaitAndRun(autoStartDelay, () =>
                {
                    current.SetActive(true);
                    ShowNextStep(true);
                }));
            }

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
            if (_active && IsHandleEvent(_currentKey, keys, out var acceptKey))
            {
                _currentKey = acceptKey;
                ShowNextStep();
            }
        }

        public void ShowCurrentStep()
        {
            if (_stepIndex >= steps.Length)
                return;
            
            steps[_stepIndex].Step.Show();
            txtCurrent.text = steps[_stepIndex].StepInfo;
        }

        public void SkipTutorial()
        {
            PlayerPrefs.SetInt(playerPrefKey, int.MaxValue);
            PlayerPrefs.Save();
                
            current.SetActive(false);
            _active = false;
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
                PlayerPrefs.SetInt(playerPrefKey, int.MaxValue);
                PlayerPrefs.Save();
                
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
        
        protected virtual void StepOnSkip(object sender, EventArgs e)
        {
            SkipTutorial();
        }
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

        public abstract string StepInfo { get; }

        #endregion

        public TutorialStepItem(T identifier)
        {
            this.identifier = identifier;
        }
    }
}