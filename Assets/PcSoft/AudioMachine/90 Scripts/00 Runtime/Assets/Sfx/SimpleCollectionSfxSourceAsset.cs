using PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Sfx.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Sfx
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.SfxMenu + "/Simple Collection Sfx Source")]
    public sealed class SimpleCollectionSfxSourceAsset : CollectionSfxSourceAsset
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