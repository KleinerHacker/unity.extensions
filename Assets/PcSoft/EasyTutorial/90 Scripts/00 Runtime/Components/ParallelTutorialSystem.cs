using System;
using System.Collections.Generic;
using System.Linq;
using UnityPrefsEx.Runtime.prefs_ex.Scripts.Runtime.Utils;

namespace PcSoft.EasyTutorial._90_Scripts._00_Runtime.Components
{
    public abstract class ParallelTutorialSystem<TS, T> : TutorialSystem<TS, T> where TS : ParallelTutorialStepItem<T> where T : Enum
    {
        private readonly ISet<T> _handledKeys = new HashSet<T>();
        
        protected ParallelTutorialSystem(T noneValue) : base(noneValue)
        {
        }

        #region Builtin Methods

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _handledKeys.Clear();
            _handledKeys.Add(_noneValue);

            var keyList = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            foreach (var key in keyList)
            {
                if (Equals(key, _noneValue))
                    continue;
                
                if (IsHandled(key))
                {
                    _handledKeys.Add(key);
                }
            }
        }

        #endregion

        public override void ResetTutorial()
        {
            base.ResetTutorial();
            
            var keyList = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            foreach (var key in keyList)
            {
                if (Equals(key, _noneValue))
                    continue;
                
                PlayerPrefsEx.SetBool(playerPrefKey + "." + key, false, true);
            }
            
            _handledKeys.Clear();
            _handledKeys.Add(_noneValue);
        }

        protected override void HandleEvent(T[] keys)
        {
            var unhandledKeys = keys.Where(x => !_handledKeys.Contains(x)).ToArray();
            if (unhandledKeys.Length <= 0)
                return;

            var key = unhandledKeys.First();
            steps.FirstOrDefault(x => Equals(key, x.Identifier))?.Step.Show();
            
            _handledKeys.Add(key);
            MarkAsHandled(key);
            if (_handledKeys.Count == Enum.GetValues(typeof(T)).Length)
            {
                _active = false;
                MarkAsFinished();
            }
        }

        private bool IsHandled(T value)
        {
            return PlayerPrefsEx.GetBool(playerPrefKey + "." + value, false);
        }

        private void MarkAsHandled(T value)
        {
            PlayerPrefsEx.SetBool(playerPrefKey + "." + value, true, true);
        }
    }
    
    [Serializable]
    public abstract class ParallelTutorialStepItem<T> : TutorialStepItem<T> where T : Enum
    {
        protected ParallelTutorialStepItem(T identifier) : base(identifier)
        {
        }
    }
}