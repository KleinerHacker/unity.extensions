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
using PcSoft.UnityWorld._90_Scripts._00_Runtime.Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PcSoft.UnityWorld._90_Scripts._00_Runtime.Components
{
    public abstract class WorldSystem<TWorld, T> : MonoBehaviour where TWorld : WorldData<T> where T : Enum
    {
        #region Inspector Data

        [SerializeField]
        protected BlendingSystem blending;

        [SerializeField]
        private TWorld[] worlds;

        [SerializeField]
        private T initialState;

        #endregion

        #region Properties

        public T State { get; protected set; }

        #endregion

        private string[] _lastScenes;

        protected WorldSystem()
        {
            worlds = ArrayUtils.CreateIdentifierArray<TWorld, T>();
        }

        #region Builtin Methods

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (AutoMasterOnPlayParameter.DoNotLoadOnAwake)
            {
                blending.HideBlendImmediately();
                return;
            }

            var world = FindWorldData();
            var scenes = world.World.Scenes.Select(x => x.Scene).ToArray();
            var activeScene = world.World.Scenes.FirstOrDefault(x => x.ActiveScene);
            ChangeScene(scenes, initialState, activeScene?.Scene);
#else
            ChangeScene(new []{splashScene}, SceneState.Splash, splashScene);
#endif
        }

        #endregion
        
        protected void ChangeScene(string[] scenes, T targetState, string activeScene, Action onFinished = null)
        {
            OnLoadingStarted(State);
            
            State = targetState;
            blending.ShowBlend(() => DoChangeScene(scenes, activeScene, () => blending.HideBlend(() =>
            {
                OnLoadingFinished(targetState);
                onFinished?.Invoke();
            })));
        }

        private void DoChangeScene(string[] scenes, string activeScene, Action onFinished)
        {
            StartCoroutine(DoChangeSceneAsync(scenes, activeScene, onFinished));
        }

        private IEnumerator DoChangeSceneAsync(string[] scenes, string activeScene, Action onFinished)
        {
            if (_lastScenes != null && _lastScenes.Length > 0)
            {
                foreach (var lastScene in _lastScenes)
                {
                    SceneManager.UnloadSceneAsync(lastScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                }
            }

            var operations = new List<AsyncOperation>();
            foreach (var scene in scenes)
            {
                var asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                asyncOperation.allowSceneActivation = false;
                
                operations.Add(asyncOperation);
            }

            while (!operations.IsReady())
            {
                blending.LoadingProgress = operations.CalculateProgress();
                yield return null;
            }

            foreach (var operation in operations)
            {
                operation.allowSceneActivation = true;
            }
            
            while (!operations.IsDone())
            {
                blending.LoadingProgress = operations.CalculateProgress();
                yield return null;
            }

            if (!string.IsNullOrEmpty(activeScene))
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(activeScene));
            }

            _lastScenes = scenes;
            onFinished?.Invoke();
        }
        
        protected TWorld FindWorldData()
        {
            return worlds.First(x => Equals(x.Identifier, initialState));
        }
        
        protected virtual void OnLoadingStarted(T oldState)
        {
        }

        protected virtual void OnLoadingFinished(T newState)
        {
        }
    }

    [Serializable]
    public abstract class WorldData<T> : IIdentifiedObject<T> where T : Enum
    {
        #region Inspector Data

        [HideInInspector]
        [SerializeField]
        private T identifier;

        [SerializeField]
        private WorldAsset world;

        #endregion

        #region Properties

        public T Identifier => identifier;

        public WorldAsset World => world;

        #endregion

        protected WorldData(T identifier)
        {
            this.identifier = identifier;
        }
    }
}