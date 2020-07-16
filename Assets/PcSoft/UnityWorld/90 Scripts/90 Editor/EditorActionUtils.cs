using System.Linq;
using PcSoft.UnityWorld._90_Scripts._00_Runtime.Assets;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PcSoft.UnityWorld._90_Scripts._90_Editor
{
    public static class EditorActionUtils
    {
        public static void LoadScenes(SceneData[] scenes)
        {
            try
            {
                LoadScene(scenes[0], 0f, false);
                if (scenes.Length > 1)
                {
                    for (var i = 0; i < scenes.Length; i++)
                    {
                        LoadScene(scenes[i], (float) i / scenes.Length, true);
                    }
                }

                var activeScene = scenes.FirstOrDefault(x => x.ActiveScene)?.Scene;
                if (!string.IsNullOrEmpty(activeScene))
                {
                    EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath(activeScene));
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        private static void LoadScene(SceneData sceneData, float progress, bool additive)
        {
            EditorUtility.DisplayProgressBar("Open World", "Load scene " + sceneData.Scene, progress);
            var scene = EditorSceneManager.OpenScene(sceneData.Scene, additive ? OpenSceneMode.Additive : OpenSceneMode.Single);
            if (sceneData.LoadingBehavior == SceneLoadingBehavior.OnlyAtRuntime)
            {
                EditorSceneManager.CloseScene(scene, false);
            }
        }
    }
}