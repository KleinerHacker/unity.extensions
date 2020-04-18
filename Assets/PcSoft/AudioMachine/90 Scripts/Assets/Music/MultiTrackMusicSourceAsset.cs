using PcSoft.AudioMachine._90_Scripts.Assets.Music.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts.Assets.Music
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.MusicMenu + "/Multi-Track Music Source")]
    public sealed class MultiTrackMusicSourceAsset : DirectMusicSourceAsset
    {
        #region Inspector Data

        [SerializeField]
        private TrackInfo[] tracks;

        #endregion

        #region Properties

        public TrackInfo[] Tracks => tracks;

        #endregion
    }
}