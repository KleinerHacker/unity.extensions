using PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Sfx.Base;
using UnityEngine;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets.Sfx
{
    [CreateAssetMenu(menuName = AudioMachineConstants.Menus.Assets.SfxMenu + "/Extended Collection Sfx Source")]
    public sealed class ExtendedCollectionSfxSourceAsset : CollectionSfxSourceAsset
    {
        #region Inspector Data

        [SerializeField]
        private DirectSfxSourceAsset[] sfxSources;

        #endregion

        #region Properties

        public DirectSfxSourceAsset[] SfxSources => sfxSources;

        #endregion
    }
}