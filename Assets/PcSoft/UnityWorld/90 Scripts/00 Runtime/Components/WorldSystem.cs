using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Utils;
using PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Utils.Extensions;
using PcSoft.ExtendedUnity._90_Scripts._90_Editor.EditorEvents;
#if UNITY_EDITOR
#endif
using PcSoft.UnityBlending._90_Scripts;
using PcSoft.UnityScene._90_Scripts._00_Runtime.Components;
using PcSoft.UnityWorld._90_Scripts._00_Runtime.Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PcSoft.UnityWorld._90_Scripts._00_Runtime.Components
{
    public abstract class WorldSystem<TWorld, T> : SceneSystem<TWorld, T> where TWorld : WorldData<T> where T : Enum
    {
        protected override void OnLoadingFinished(T newState, TWorld world)
        {
            var scene = world?.World?.Scenes.FirstOrDefault(x => x.ActiveScene);
            if (scene == null)
                return;

            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scene.Scene));
        }
    }

    [Serializable]
    public abstract class WorldData<T> : SceneData<T>, IIdentifiedObject<T> where T : Enum
    {
        #region Inspector Data

        [SerializeField]
        private WorldAsset world;

        #endregion

        #region Properties

        public WorldAsset World => world;

        public override string[] Scenes => base.Scenes.Concat(
            world.Scenes
                .Where(x => x.LoadingBehavior != (Application.isEditor ? SceneLoadingBehavior.OnlyAtRuntime : SceneLoadingBehavior.OnlyInEditor))
                .Select(x => x.Scene)
        ).ToArray();

        #endregion

        protected WorldData(T identifier) : base(identifier)
        {
        }
    }
}