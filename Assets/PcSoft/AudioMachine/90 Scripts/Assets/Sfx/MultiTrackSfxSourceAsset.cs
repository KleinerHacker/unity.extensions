using PcSoft.AudioMachine._90_Scripts.Assets.Sfx.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts.Assets.Sfx
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.SfxMenu + "/Multi-Track Sfx Source")]
    public sealed class MultiTrackSfxSourceAsset : DirectSfxSourceAsset
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