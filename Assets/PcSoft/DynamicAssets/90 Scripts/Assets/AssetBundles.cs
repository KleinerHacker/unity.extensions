using PcSoft.DynamicAssets._90_Scripts.Assets.Types;
using UnityEngine;

namespace PcSoft.DynamicAssets._90_Scripts.Assets
{
    [CreateAssetMenu(menuName = DynamicAssetsConstants.Menus.Assets.RootMenu + "/Bundles")]
    public sealed class AssetBundles : AssetBase<ExtendedPathInfo>
    {
        #region Static Area

        private static AssetBundles _instance;

        public static AssetBundles Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<AssetBundles>("");
                }

                return _instance;
            }
        }

        #endregion
    }
}