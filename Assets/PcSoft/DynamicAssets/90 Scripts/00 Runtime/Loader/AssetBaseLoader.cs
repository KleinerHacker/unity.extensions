using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace PcSoft.DynamicAssets._90_Scripts._00_Runtime.Loader
{
    public abstract class AssetBaseLoader
    {
        private readonly IDictionary<Type, Object[]> _assets = new Dictionary<Type, Object[]>();

        public void LoadAssets(Type[] types, string path, bool throwIfEmpty = false)
        {
            var objects = LoadFrom(types, path);
            WriteObjectsToDictionary(types, path, throwIfEmpty, objects);
        }

        public void LoadAssets(Type type, string path, bool throwIfEmpty = false)
        {
            LoadAssets(new []{type}, path, throwIfEmpty);
        }

        public void LoadAssets<TA>(string path, bool throwIfEmpty = false)
        {
            LoadAssets(typeof(TA), path, throwIfEmpty);
        }

        public void LoadAssetsAsync(Type[] types, string path, bool throwIfEmpty = false)
        {
            LoadFromAsync(types, path, objects => WriteObjectsToDictionary(types, path, throwIfEmpty, objects));
        }

        public void LoadAssetsAsync(Type type, string path, bool throwIfEmpty = false)
        {
            LoadAssetsAsync(new []{type}, path, throwIfEmpty);
        }

        public void LoadAssetsAsync<TA>(string path, bool throwIfEmpty = false)
        {
            LoadAssetsAsync(typeof(TA), path, throwIfEmpty);
        }

        public int GetCountOfAssets(Type type) => _assets.ContainsKey(type) ? _assets[type].Length : 0;

        public int GetCountOfAssets<TA>() => GetCountOfAssets(typeof(TA));

        public bool HasAsset(Type type) => _assets.ContainsKey(type);

        public bool HasAsset<TA>() => HasAsset(typeof(TA));

        public TA GetAsset<TA>(bool throwIfNotOne = false) where TA : Object
        {
            return (TA) GetAsset(typeof(TA), throwIfNotOne);
        }

        public Object GetAsset(Type type, bool throwIfNotOne = false)
        {
            var assets = GetAssets(type);
            if (throwIfNotOne && assets.Length != 1)
                throw new InvalidOperationException("No or more than one asset of type " + type.FullName + " was found");

            return assets.FirstOrDefault();
        }

        public TA[] GetAssets<TA>(bool throwIfEmpty = false) where TA : Object
        {
            return GetAssets(typeof(TA), throwIfEmpty).Cast<TA>().ToArray();
        }

        public Object[] GetAssets(Type type, bool throwIfEmpty = false)
        {
            if (!_assets.ContainsKey(type))
            {
                if (throwIfEmpty)
                    throw new InvalidOperationException("No asset of type " + type.FullName + " was found");

                return new Object[0];
            }

            return _assets[type];
        }

        protected abstract IDictionary<Type, Object[]> LoadFrom(Type[] types, string path);

        protected abstract void LoadFromAsync(Type[] types, string path, AsyncAnswer answer);

        protected delegate void AsyncAnswer(IDictionary<Type, Object[]> answer);

        private void WriteObjectsToDictionary(Type[] types, string path, bool throwIfEmpty, IDictionary<Type, Object[]> objects)
        {
            if (objects.Count <= 0)
            {
                if (throwIfEmpty)
                    throw new InvalidOperationException("No asset of types " + string.Join(",", types.Select(x => x.FullName)) + " found at " + path);

                return;
            }

            foreach (var type in objects.Keys)
            {
                if (_assets.ContainsKey(type))
                {
                    var list = _assets[type].ToList();
                    list.AddRange(objects[type]);
                    _assets[type] = list.Distinct().ToArray();
                }
                else
                {
                    _assets.Add(type, objects[type]);
                }
            }
        }
    }
}