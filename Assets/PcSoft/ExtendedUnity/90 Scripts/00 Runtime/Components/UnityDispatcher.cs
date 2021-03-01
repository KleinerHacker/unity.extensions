using System;
using System.Collections.Generic;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Components
{
    [AddComponentMenu(ExtendedUnityConstants.Root + "/Unity Dispatcher")]
    public class UnityDispatcher : SearchingSingletonBehavior<UnityDispatcher>
    {
        private readonly IList<Action> _runUpdateList = new List<Action>();
        private readonly IList<Action> _runLateUpdateList = new List<Action>();
        private readonly IList<Action> _runFixedUpdateList = new List<Action>();

        public void RunLaterInUpdate(Action action)
        {
            AddAction(action, _runUpdateList);
        }
        
        public void RunLaterInLateUpdate(Action action)
        {
            AddAction(action, _runLateUpdateList);
        }
        
        public void RunLaterInFixedUpdate(Action action)
        {
            AddAction(action, _runFixedUpdateList);
        }

        #region Builtin Methods

        private void Update()
        {
            RunActions(_runUpdateList);
        }

        private void LateUpdate()
        {
            RunActions(_runLateUpdateList);
        }

        private void FixedUpdate()
        {
            RunActions(_runFixedUpdateList);
        }

        #endregion

        private void AddAction(Action action, IList<Action> actions)
        {
            lock (actions)
            {
                actions.Add(action);
            }
        }

        private void RunActions(IList<Action> actions)
        {
            lock (actions)
            {
                foreach (var action in actions)
                {
                    action();
                }
                
                actions.Clear();
            }
        }
    }
}