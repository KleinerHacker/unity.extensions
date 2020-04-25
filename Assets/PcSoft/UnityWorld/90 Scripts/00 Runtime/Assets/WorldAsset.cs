using System;
using UnityEngine;

namespace Assets.PcSoft.UnityWorld._90_Scripts._00_Runtime.Assets
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

        [SerializeField]
        private string scene;

        [SerializeField]
        private bool activeScene;

        [SerializeField]
        private bool doNotLoadInEditor;

        [SerializeField]
        private WorldSceneGroup group = WorldSceneGroup.None;

        #endregion

        #region Properties

        public string Scene => scene;

        public bool ActiveScene => activeScene;

        public bool DoNotLoadInEditor => doNotLoadInEditor;

        public WorldSceneGroup Group => group;

        #endregion
    }

    public enum WorldSceneGroup
    {
        None,
        One, 
        Two, 
        Three
    }
}