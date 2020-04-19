using System;
using PcSoft.DynamicAssets._90_Scripts.Assets.Types;
using UnityEngine;

namespace PcSoft.DynamicAssets._90_Scripts.Assets
{
    [CreateAssetMenu(menuName = DynamicAssetsConstants.Menus.Assets.RootMenu + "/Resources")]
    public sealed class AssetResources : AssetBase
    {
        #region Static Area

        private static AssetResources _instance;

        public static AssetResources Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<AssetResources>("");
                }

                return _instance;
            }
        }

        #endregion
    }
}