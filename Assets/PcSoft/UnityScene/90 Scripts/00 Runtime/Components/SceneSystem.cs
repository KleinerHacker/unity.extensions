using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Utils;
using PcSoft.ExtendedUnity._90_Scripts.EditorEvents;
using PcSoft.ExtendedUnity._90_Scripts.Utils.Extensions;
using PcSoft.UnityBlending._90_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
#endif

namespace PcSoft.UnityScene._90_Scripts._00_Runtime.Components
{
    public abstract class SceneSystem<TScene, T> : MonoBehaviour where TScene : SceneData<T> where T : Enum
    {
        #region Inspector Data

        [SerializeField]
        private BlendingSystem blending;

        [SerializeField]
        private TScene[] scenes;

        [SerializeField]
        private T initialState;

        #endregion

        public T State { get; protected set; }

        protected SceneSystem()
        {
            scenes = ArrayUtils.CreateIdentifierArray<TScene, T>();
        }

        #region Builtin Methods

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (AutoMasterOnPlayParameter.DoNotLoadOnAwake)
            {
                blending.HideBlendImmediately();
                return;
            }

            State = initialState;
            LoadScene(State, doNotUnload: true);
#else
            LoadScene(SceneState, doNotUnload:true);
#endif
        }

        #endregion

        #region Private Methods

        private void LoadScene(T state, Action onFinished = null, bool doNotUnload = false)
        {
            OnLoadingStarted();

            string[] oldScenes = null;
            if (!doNotUnload)
            {
                var oldData = FindSceneData(State);
                oldScenes = oldData.Scenes;
            }

            var newData = FindSceneData(state);
            var newScenes = newData.Scenes;

            blending.ShowBlend(() =>
            {
                StartCoroutine(ChangeScenes(oldScenes, newScenes, () =>
                {
                    blending.HideBlend(() =>
                    {
                        OnLoadingFinished();

                        State = state;
                        onFinished?.Invoke();
                    });
                }));
            });
        }

        private TScene FindSceneData(T state)
        {
            return scenes.First(item => Equals(item.Identifier, state));
        }

        private IEnumerator ChangeScenes(string[] oldScenes, string[] newScenes, Action onFinished)
        {
            if (oldScenes != null && oldScenes.Length > 0)
            {
                foreach (var oldScene in oldScenes)
                {
                    SceneManager.UnloadSceneAsync(oldScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);                    
                }
            }

            var operations = new List<AsyncOperation>();
            foreach (var newScene in newScenes)
            {
                var operation = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
                operation.allowSceneActivation = false;
                
                operations.Add(operation);
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

            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(newScenes[0]));

            onFinished?.Invoke();
        }

        #endregion

        protected virtual void OnLoadingStarted()
        {
        }

        protected virtual void OnLoadingFinished()
        {
        }
    }

    [Serializable]
    public abstract class SceneData<T> : IIdentifiedObject<T> where T : Enum
    {
        #region Inspector Data

        [HideInInspector]
        [SerializeField]
        private T identifier;

        [Scene]
        [SerializeField]
        private string[] scenes;

        #endregion

        #region Properties

        public T Identifier => identifier;

        public string[] Scenes => scenes;

        #endregion

        protected SceneData(T identifier)
        {
            this.identifier = identifier;
        }
    }
}