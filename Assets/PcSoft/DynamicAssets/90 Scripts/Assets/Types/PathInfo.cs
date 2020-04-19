using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PcSoft.DynamicAssets._90_Scripts.Assets.Types
{
    [Serializable]
    public class PathInfo
    {
        #region Inspector Data

        [SerializeField]
        private string type;

        [SerializeField]
        private string path;

        #endregion

        #region Properties

        public Type Type => Type.GetType(type);
        
        public string Path => path;

        #endregion
    }

    [Serializable]
    public class ExtendedPathInfo : PathInfo
    {
        #region Inspector Data

        [SerializeField]
        private string filePattern;

        #endregion

        #region Properties

        public string FilePattern => filePattern;

        #endregion
    }
}