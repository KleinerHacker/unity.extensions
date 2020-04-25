using System;
using UnityEngine;

namespace PcSoft.UnityBlending._90_Scripts
{
    public abstract class BlendingSystem : MonoBehaviour
    {
        public float LoadingProgress { get; set; }
        
        public abstract void ShowBlend(Action onFinished = null);
        public abstract void ShowBlendImmediately();
        public abstract void HideBlend(Action inFinished = null);
        public abstract void HideBlendImmediately();
    }
}