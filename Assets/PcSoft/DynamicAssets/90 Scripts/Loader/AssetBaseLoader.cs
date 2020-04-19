using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.DynamicAssets._90_Scripts.Assets;
using PcSoft.DynamicAssets._90_Scripts.Assets.Types;

namespace PcSoft.DynamicAssets._90_Scripts.Loader
{
    public abstract class AssetBaseLoader<T,TP> where T : AssetBase<TP> where TP : PathInfo
    {
        public bool IsInitialized { get; protected set; }
        protected abstract T AssetInstance { get; }

        protected readonly IDictionary<Type, UnityEngine.Object[]> _assets = new Dictionary<Type, UnityEngine.Object[]>();
        protected bool _lazyLoading;

        public void Initialize(bool lazyLoading = false)
        {
            if (IsInitialized)
                throw new InvalidOperationException("Already initialized");

            if (!lazyLoading)
            {
                Load();
            }

            _lazyLoading = lazyLoading;
            IsInitialized = true;
        }

        public TA GetAsset<TA>() where TA : UnityEngine.Object
        {
            return GetAllAssets<TA>().FirstOrDefault();
        }

        public TA[] GetAllAssets<TA>() where TA : UnityEngine.Object
        {
            CheckIsInitialized();
            CheckType(typeof(TA));
            
            return _assets[typeof(TA)].Cast<TA>().ToArray();
        }

        private void Load()
        {
            _assets.Clear();

            foreach (var path in AssetInstance.Paths)
            {
                var assets = LoadAssets(path);
                _assets.Add(path.Type, assets);
            }
        }

        private void CheckIsInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Not initialized yet");
        }

        private void CheckType(Type type)
        {
            if (!_assets.ContainsKey(type))
            {
                if (_lazyLoading)
                {
                    var path = AssetInstance.Paths.FirstOrDefault(x => x.Type == type);
                    if (path == null)
                        throw new InvalidOperationException("Unable to find asset type " + type.FullName);

                    var assets = LoadAssets(path);
                    _assets.Add(type, assets);
                }
                else
                    throw new InvalidOperationException("Unable to find asset type " + type.FullName);
            }
        }

        protected abstract UnityEngine.Object[] LoadAssets(TP path);
    }
}