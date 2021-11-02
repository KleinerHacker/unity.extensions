using UnityEngine;

namespace PcSoft.UnityTooling._90_Scripts._90_Editor.Assets.Misc
{
    [CreateAssetMenu(menuName = UnityToolingConstants.Menu.Asset.MiscMenu + "/Unity Package")]
    public sealed class UnityPackageAsset : ScriptableObject
    {
        #region Inspector Data

        [SerializeField]
        private string packageName;

        [SerializeField]
        private string[] assetPaths;

        #endregion

        #region Propertes

        public string PackageName => packageName;

        public string[] AssetPaths => assetPaths;

        #endregion
    }
}