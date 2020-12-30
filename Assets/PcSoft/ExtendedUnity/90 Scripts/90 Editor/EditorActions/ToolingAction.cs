using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PcSoft.ExtendedUnity._90_Scripts._90_Editor.Assets.Misc;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PcSoft.ExtendedUnity._90_Scripts._90_Editor.EditorActions
{
    public static class ToolingAction
    {
        [MenuItem("Tools/Data/Delete All Player Prefs")]
        public static void DeleteAllPrefs()
        {
            if (EditorUtility.DisplayDialog("Delete All Player Prefs", "You are sure to delete all player prefs?", "Yes", "No"))
            {
                Debug.Log("Delete all player prefs");
                PlayerPrefs.DeleteAll();
            }
        }
        
        [MenuItem("Tools/Data/Delete All Persistence Data")]
        public static void DeleteAllSaveData()
        {
            if (EditorUtility.DisplayDialog("Delete All Persistence Data", "You are sure to delete all persistence data?", "Yes", "No"))
            {
                Debug.Log("Delete all persistence data");
                
                foreach (var file in Directory.EnumerateFiles(Application.persistentDataPath))
                {
                    Debug.Log("> Delete file " + file);
                    File.Delete(file);
                }

                foreach (var directory in Directory.EnumerateDirectories(Application.persistentDataPath))
                {
                    Debug.Log("> Delete directory " + directory);
                    Directory.Delete(directory, true);
                }
            }
        }

        [MenuItem("Tools/Unity Packages/Auto Generate")]
        public static void AutoGenerateUnityPackages()
        {
            var folder = EditorUtility.OpenFolderPanel("Auto Generate Unity Packages", null, null);
            if (string.IsNullOrEmpty(folder))
                return;

            try
            {
                var packageAssets = FindAssetsByType<UnityPackageAsset>();
                Debug.Log("Found: " + packageAssets.Count);
                for (var i = 0; i < packageAssets.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Auto Generate Unity Packages", packageAssets[i].PackageName, (float)i / packageAssets.Count);
                
                    var packageAsset = packageAssets[i];
                    AssetDatabase.ExportPackage(packageAsset.AssetPaths, folder + "/" + packageAsset.PackageName + ".unitypackage", ExportPackageOptions.Recurse);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            Process.Start(folder);
        }
        
        private static List<T> FindAssetsByType<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .Where(asset => asset != null).ToList();
        }
    }
}