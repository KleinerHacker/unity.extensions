using System;
using UnityEngine;

namespace PcSoft.UnityBlending._90_Scripts._00_Runtime
{
    public abstract class BlendingSystem : MonoBehaviour
    {
        public abstract float LoadingProgress { get; set; }
        
        public abstract void ShowBlend(Action onFinished = null);
        public abstract void ShowBlendImmediately();
        public abstract void HideBlend(Action onFinished = null);
        public abstract void HideBlendImmediately();
    }
}