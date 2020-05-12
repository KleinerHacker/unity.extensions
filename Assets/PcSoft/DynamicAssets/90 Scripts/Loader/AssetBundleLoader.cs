using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PcSoft.DynamicAssets._90_Scripts.Loader
{
    public sealed class AssetBundleLoader : AssetBaseLoader
    {
        public static AssetBundleLoader Instance { get; } = new AssetBundleLoader();

        private AssetBundleLoader()
        {
        }

        protected override IDictionary<Type, Object[]> LoadFrom(Type[] types, string path)
        {
            var dict = new Dictionary<Type, Object[]>();
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var assetBundle = AssetBundle.LoadFromFile(file);
                foreach (var type in types)
                {
                    if (dict.ContainsKey(type))
                    {
                        var list = dict[type].ToList();
                        list.AddRange(assetBundle.LoadAllAssets(type));
                        dict[type] = list.ToArray();
                    }
                    else
                    {
                        dict.Add(type, assetBundle.LoadAllAssets(type));
                    }
                }
                assetBundle.Unload(false);
            }

            return dict;
        }

        protected override void LoadFromAsync(Type[] types, string path, AsyncAnswer answer)
        {
            var dict = new Dictionary<Type, Object[]>();
            var requests = new List<AsyncOperation>();
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var fileRequest = AssetBundle.LoadFromFileAsync(file);
                fileRequest.completed += operation =>
                {
                    foreach (var type in types)
                    {
                        var bundleRequest = fileRequest.assetBundle.LoadAllAssetsAsync(type);
                        bundleRequest.completed += op =>
                        {
                            if (dict.ContainsKey(type))
                            {
                                var list = dict[type].ToList();
                                list.AddRange(bundleRequest.allAssets);
                                dict[type] = list.ToArray();
                            }
                            else
                            {
                                dict.Add(type, bundleRequest.allAssets);
                            }
                        };
                        requests.Add(bundleRequest);
                    }

                    fileRequest.assetBundle.Unload(false);
                };
                requests.Add(fileRequest);
            }

            Task.Run(() =>
            {
                while (!requests.All(x => x.isDone))
                {
                    Thread.Sleep(100);
                }
                
                answer.Invoke(dict);
            });
        }
    }
}