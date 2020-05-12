#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PcSoft.DynamicAssets._90_Scripts.Loader
{
    public sealed class AssetEditorLoader : AssetBaseLoader
    {
        protected override IDictionary<Type, Object[]> LoadFrom(Type[] types, string path)
        {
            return types.ToDictionary(x => x, x => AssetDatabase.LoadAllAssetsAtPath(path));
        }

        protected override void LoadFromAsync(Type[] types, string path, AsyncAnswer answer)
        {
            answer.Invoke(LoadFrom(types, path));
        }
    }
}
#endif