using PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Sfx.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Sfx
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.SfxMenu + "/Simple Sfx Source")]
    public sealed class SimpleSfxSourceAsset : DirectSfxSourceAsset
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