using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Components
{
    public abstract class SingletonUIBehavior<T> : UIBehaviour where T : SingletonUIBehavior<T>
    {
        public static T Singleton { get; private set; }

        protected override void OnEnable()
        {
            Singleton = (T) this;
        }

        protected override void OnDisable()
        {
            Singleton = null;
        }
    }
}