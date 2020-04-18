using PcSoft.AudioMachine._90_Scripts.Assets.Music.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts.Assets.Music
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.MusicMenu + "/Extended Collection Music Source")]
    public sealed class ExtendedCollectionMusicSourceAsset : CollectionMusicSourceAsset
    {
        #region Inspector Data

        [SerializeField]
        private DirectMusicSourceAsset[] musicSources;

        #endregion

        #region Properties

        public DirectMusicSourceAsset[] MusicSources => musicSources;

        #endregion
    }
}