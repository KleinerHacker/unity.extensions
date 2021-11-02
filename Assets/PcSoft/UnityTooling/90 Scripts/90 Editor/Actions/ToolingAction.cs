using System.IO;
using UnityEditor;
using UnityEngine;

namespace PcSoft.UnityTooling._90_Scripts._90_Editor.Actions
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
    }
}