using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.DynamicAssets._90_Scripts.Assets;
using PcSoft.DynamicAssets._90_Scripts.Assets.Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PcSoft.DynamicAssets._90_Scripts.Loader
{
    public sealed class AssetResourcesLoader : AssetBaseLoader<AssetResources, PathInfo>
    {
        public static AssetResourcesLoader Instance { get; } = new AssetResourcesLoader();
        
        protected override AssetResources AssetInstance => AssetResources.Instance;
        
        private AssetResourcesLoader()
        {
        }

        protected override Object[] LoadAssets(PathInfo path)
        {
            return Resources.LoadAll(path.Path, path.Type);
        }
    }
}