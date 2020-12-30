using PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Music.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Music
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.MusicMenu + "/Simple Collection Music Source")]
    public sealed class SimpleCollectionMusicSourceAsset : CollectionMusicSourceAsset
    {
        #region Inspector Data

        [SerializeField]
        private AudioClip[] clips;

        #endregion

        #region Properties

        public AudioClip[] Clips => clips;

        #endregion
    }
}