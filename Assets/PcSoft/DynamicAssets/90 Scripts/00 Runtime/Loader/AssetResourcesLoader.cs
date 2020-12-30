using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PcSoft.DynamicAssets._90_Scripts._00_Runtime.Loader
{
    public sealed class AssetResourcesLoader : AssetBaseLoader
    {
        public static AssetResourcesLoader Instance { get; } = new AssetResourcesLoader();
        
        private AssetResourcesLoader()
        {
        }

        protected override IDictionary<Type, Object[]> LoadFrom(Type[] types, string path)
        {
            return types.ToDictionary(x => x, x => Resources.LoadAll(path, x));
        }

        protected override void LoadFromAsync(Type[] types, string path, AsyncAnswer answer)
        {
            throw new NotSupportedException("Async loading is not supported by resources");
        }
    }
}