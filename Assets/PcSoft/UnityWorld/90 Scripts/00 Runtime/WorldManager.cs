using System;
using System.Collections.Generic;
using System.Linq;
using Assets.PcSoft.UnityWorld._90_Scripts._00_Runtime.Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.PcSoft.UnityWorld._90_Scripts._00_Runtime
{
    public static class WorldManager
    {
        public static void LoadWorld(WorldAsset world, LoadWorldMode mode = LoadWorldMode.Single)
        {
            switch (mode)
            {
                case LoadWorldMode.Additive:
                    SceneManager.LoadScene(world.Scenes[0].Scene, LoadSceneMode.Additive);
                    break;
                case LoadWorldMode.Single:
                    SceneManager.LoadScene(world.Scenes[0].Scene, LoadSceneMode.Single);
                    break;
                default:
                    throw new NotImplementedException();
            }

            for (var i = 1; i < world.Scenes.Length; i++)
            {
                SceneManager.LoadScene(world.Scenes[i].Scene);
            }

            var activeScene = world.Scenes.FirstOrDefault(x => x.ActiveScene);
            if (activeScene != null)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(activeScene.Scene));
            }
        }

        public static AsyncOperation[] LoadWorldAsync(WorldAsset world, LoadWorldMode mode = LoadWorldMode.Single)
        {
            var asyncList = new List<AsyncOperation>();

            AsyncOperation firstAsyncOperation;
            switch (mode)
            {
                case LoadWorldMode.Additive:
                    firstAsyncOperation = SceneManager.LoadSceneAsync(world.Scenes[0].Scene, LoadSceneMode.Additive);
                    break;
                case LoadWorldMode.Single:
                    firstAsyncOperation = SceneManager.LoadSceneAsync(world.Scenes[0].Scene, LoadSceneMode.Single);
                    break;
                default:
                    throw new NotImplementedException();
            }
            asyncList.Add(firstAsyncOperation);
            SetActiveSceneAsync(world.Scenes[0], firstAsyncOperation);

            for (var i = 1; i < world.Scenes.Length; i++)
            {
                var asyncOperation = SceneManager.LoadSceneAsync(world.Scenes[i].Scene);
                
                asyncList.Add(asyncOperation);
                SetActiveSceneAsync(world.Scenes[i], asyncOperation);
            }

            return asyncList.ToArray();
        }

        public static AsyncOperation[] UnloadWorldAsync(WorldAsset world)
        {
            var asyncList = new List<AsyncOperation>();
            foreach (var scene in world.Scenes)
            {
                var asyncOperation = SceneManager.UnloadSceneAsync(scene.Scene);
                asyncList.Add(asyncOperation);
            }

            return asyncList.ToArray();
        } 

        private static void SetActiveSceneAsync(SceneData sceneData, AsyncOperation asyncOperation)
        {
            if (sceneData.ActiveScene)
            {
                asyncOperation.completed += operation => { SceneManager.SetActiveScene(SceneManager.GetSceneByPath(sceneData.Scene)); };
            }
        }
    }

    public enum LoadWorldMode
    {
        Additive,
        Single
    }
}