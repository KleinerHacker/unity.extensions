using System;
using System.Collections.Generic;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Components
{
    [AddComponentMenu(ExtendedUnityConstants.Root + "/Unity Dispatcher")]
    public sealed class UnityDispatcher : ObserverSingletonBehavior<UnityDispatcher>
    {
        private readonly IList<Action> _runList = new List<Action>();

        public void RunLater(Action action)
        {
            AddAction(action, _runList);
        }

        #region Builtin Methods

        private void LateUpdate()
        {
            RunActions(_runList);
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