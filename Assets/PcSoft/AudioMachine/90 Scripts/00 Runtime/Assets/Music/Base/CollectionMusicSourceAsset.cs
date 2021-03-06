using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Music.Base
{
    public abstract class CollectionMusicSourceAsset : MusicSourceAsset
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