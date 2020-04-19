using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PcSoft.DynamicAssets._90_Scripts.Assets;
using PcSoft.DynamicAssets._90_Scripts.Assets.Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PcSoft.DynamicAssets._90_Scripts.Loader
{
    public sealed class AssetBundleLoader : AssetBaseLoader<AssetBundles, ExtendedPathInfo>
    {
        public static AssetBundleLoader Instance { get; } = new AssetBundleLoader();
        
        protected override AssetBundles AssetInstance => AssetBundles.Instance;
        
        private readonly IDictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();

        private AssetBundleLoader()
        {
        }
        
        public AssetBundleCreateRequest[] InitializeAsync()
        {
            if (IsInitialized)
                throw new InvalidOperationException("Already initialized");

            var asyncList = new List<AssetBundleCreateRequest>();
            foreach (var path in AssetInstance.Paths)
            {
                foreach (var file in Directory.EnumerateFiles(path.Path, path.FilePattern))
                {
                    var request = AssetBundle.LoadFromFileAsync(file);
                    request.completed += operation => _bundles.Add(file, request.assetBundle);
                    
                    asyncList.Add(request);
                }
            }

            _lazyLoading = true;
            IsInitialized = true;
            
            return asyncList.ToArray();
        }

        protected override Object[] LoadAssets(ExtendedPathInfo path)
        {
            var assets = new List<Object>();
            foreach (var file in Directory.EnumerateFiles(path.Path, path.FilePattern))
            {
                AssetBundle assetBundle;
                if (!_bundles.ContainsKey(file))
                {
                    assetBundle = AssetBundle.LoadFromFile(file);
                    _bundles.Add(file, assetBundle);
                }
                else
                {
                    assetBundle = _bundles[file];
                }

                assets.AddRange(assetBundle.LoadAllAssets(path.Type));
            }

            return assets.ToArray();
        }
    }
}