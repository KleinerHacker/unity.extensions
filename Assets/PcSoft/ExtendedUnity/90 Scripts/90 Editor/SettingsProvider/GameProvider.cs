using System;
using System.Collections.Generic;
using System.IO;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Extra;
using PcSoft.ExtendedUnity._90_Scripts.Assets;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PcSoft.ExtendedUnity._90_Scripts.SettingsProvider
{
    public class GameProvider : UnityEditor.SettingsProvider
    {
        #region Static Area

        [SettingsProvider]
        public static UnityEditor.SettingsProvider CreateGameSettingsProvider()
        {
            return new GameProvider();
        }

        #endregion

        private SerializedObject _settings;
        private SerializedProperty _masterSceneProperty;
        private SerializedProperty _loadProperty;

        public GameProvider() :
            base("Project/Game", SettingsScope.Project, new[] {"Game", "Settings", "Master", "Scene"})
        {
        }

        #region Builtin Methods

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settings = GameSettings.SerializedSingleton;
            _masterSceneProperty = _settings.FindProperty("masterScene");
            _loadProperty = _settings.FindProperty("loadMasterIfNeeded");
        }

        public override void OnGUI(string searchContext)
        {
            _settings.Update();
            
            EditorGUILayout.PropertyField(_masterSceneProperty, new GUIContent("Master Scene"));
            EditorGUILayout.PropertyField(_loadProperty, new GUIContent("Load Master Scene if needed"));

            _settings.ApplyModifiedProperties();
        }

        #endregion
    }

    internal sealed class GameSettings : ScriptableObject
    {
        private const string Path = "Assets/game.asset";

        public static GameSettings Singleton
        {
            get
            {
                var settings = AssetDatabase.LoadAssetAtPath<GameSettings>(Path);
                if (settings == null)
                {
                    Debug.Log("Unable to find game settings, create new");
                    
                    settings = CreateInstance<GameSettings>();
                    AssetDatabase.CreateAsset(settings, Path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                return settings;
            }
        }

        public static SerializedObject SerializedSingleton => new SerializedObject(Singleton);

        #region Inspector Data

        [Scene]
        [SerializeField]
        private string masterScene;

        [SerializeField]
        private bool loadMasterIfNeeded = true;

        #endregion

        #region Properties

        public string MasterScene => masterScene;

        public bool LoadMasterIfNeeded => loadMasterIfNeeded;

        #endregion
    }
}