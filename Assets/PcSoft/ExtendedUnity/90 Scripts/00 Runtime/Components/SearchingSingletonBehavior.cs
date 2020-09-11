using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Components
{
    public abstract class SearchingSingletonBehavior<T> : MonoBehaviour where T : SearchingSingletonBehavior<T>
    {
        public static T Singleton => Resources.FindObjectsOfTypeAll<T>()[0];
    }
}