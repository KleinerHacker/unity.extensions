#if UNITY_EDITOR
using PcSoft.ExtendedUnity._90_Scripts.SettingsProvider;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PcSoft.ExtendedUnity._90_Scripts.EditorEvents
{
    [InitializeOnLoad]
    public static class AutoMasterOnPlayEvent
    {
        static AutoMasterOnPlayEvent()
        {
            Debug.Log("[EDITOR] Auto Master initialization...");
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (string.IsNullOrEmpty(GameSettings.Singleton.MasterScene) || !GameSettings.Singleton.LoadMasterIfNeeded)
                    return;
                
                Debug.Log("[EDITOR] Auto Master triggered...");
                if (!SceneManager.GetSceneByPath(GameSettings.Singleton.MasterScene).IsValid())
                {
                    AutoMasterOnPlayParameter.DoNotLoadOnAwake = true;
                    
                    Debug.Log("[EDITOR] Auto Master loading...");
                    SceneManager.LoadScene(GameSettings.Singleton.MasterScene, LoadSceneMode.Additive);
                }
                else
                {
                    AutoMasterOnPlayParameter.DoNotLoadOnAwake = false;
                }
            }
        }
    }

    public static class AutoMasterOnPlayParameter
    {
        #region Properties

        public static bool DoNotLoadOnAwake { get; set; } = false;

        #endregion
    }
}
#endif