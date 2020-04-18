using PcSoft.AudioMachine._90_Scripts.Assets.Music.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts.Assets.Music
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.MusicMenu + "/Music Source")]
    public sealed class SimpleMusicSourceAsset : DirectMusicSourceAsset
    {
        #region Inspector Data

        [SerializeField]
        private AudioClip clip;

        #endregion

        #region Properties

        public AudioClip Clip => clip;

        #endregion
    }
}