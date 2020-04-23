using UnityEngine;

namespace PcSoft.ExtendedAnimation._90_Scripts.Utils
{
    public sealed class AnimationRunner
    {
        private readonly MonoBehaviour _behaviour;

        internal AnimationRunner(MonoBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public void Stop()
        {
            _behaviour.StopAllCoroutines();
        }
    }
}