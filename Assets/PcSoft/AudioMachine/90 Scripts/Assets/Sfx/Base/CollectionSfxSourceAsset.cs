using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts.Assets.Sfx.Base
{
    public abstract class CollectionSfxSourceAsset : SfxSourceAsset
    {
        #region Inspector Data

        [SerializeField]
        private CollectionAudioBehavior playBehavior = CollectionAudioBehavior.PlayRandomNoDoublet;

        #endregion

        #region Properties

        public CollectionAudioBehavior PlayBehavior => playBehavior;

        #endregion
    }
}