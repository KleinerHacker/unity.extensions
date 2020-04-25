using System;

namespace PcSoft.SaveGame._90_Scripts.Types
{
    public sealed class AsyncSerialization
    {
        private float _progress;

        public float Progress
        {
            get => _progress;
            internal set
            {
                if (IsDone)
                    return;
                
                _progress = value;
            }
        }

        public bool IsDone => Progress >= 1f;

        public event EventHandler OnCompleted;

        internal void Completed()
        {
            Progress = 1f;
            OnCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}