using PcSoft.DynamicAssets._90_Scripts.Assets.Types;
using UnityEngine;

namespace PcSoft.DynamicAssets._90_Scripts.Assets
{
    public abstract class AssetBase<T> : ScriptableObject where T : PathInfo
    {
        #region Inspector Data

        [SerializeField]
        private T[] paths;

        #endregion

        #region Properties

        public T[] Paths => paths;

        #endregion
    }

    public abstract class AssetBase : AssetBase<PathInfo>
    {
    }
}