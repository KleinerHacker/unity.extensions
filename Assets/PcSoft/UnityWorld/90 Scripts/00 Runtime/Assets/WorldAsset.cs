using System;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra;
using UnityEngine;
using UnityEngine.Serialization;

namespace PcSoft.UnityWorld._90_Scripts._00_Runtime.Assets
{
    [CreateAssetMenu(menuName = UnityWorldConstants.Menus.Assets.RootMenu + "/World")]
    public sealed class WorldAsset : ScriptableObject
    {
        #region Inspector Data

        [SerializeField]
        private SceneData[] scenes;

        #endregion

        #region Properties

        public SceneData[] Scenes => scenes;

        #endregion
    }
    
    [Serializable]
    public sealed class SceneData
    {
        #region Inspector Data

        [Scene]
        [SerializeField]
        private string scene;

        [SerializeField]
        private bool activeScene;

        [SerializeField]
        private SceneLoadingBehavior loadingBehavior = SceneLoadingBehavior.Always;

        [SerializeField]
        private WorldSceneGroup group = WorldSceneGroup.None;

        #endregion

        #region Properties

        public string Scene => scene;

        public bool ActiveScene => activeScene;

        public SceneLoadingBehavior LoadingBehavior => loadingBehavior;

        public WorldSceneGroup Group => group;

        #endregion
    }

    public enum WorldSceneGroup
    {
        None,
        One, 
        Two, 
        Three,
        All
    }

    public enum SceneLoadingBehavior
    {
        Always,
        OnlyInEditor,
        OnlyAtRuntime
    }
}