using UnityEngine;
using UnityEngine.EventSystems;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Components
{
    public abstract class SearchingSingletonUIBehavior<T> : UIBehaviour where T : SearchingSingletonUIBehavior<T>
    {
        public static T Singleton => FindObjectOfType<T>();
    }
}