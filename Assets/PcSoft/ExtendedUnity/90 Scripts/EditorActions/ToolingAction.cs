using UnityEditor;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts.EditorActions
{
    public static class ToolingAction
    {
        [MenuItem("Tools/Player Prefs/Delete All")]
        public static void DeleteAllPrefs()
        {
            if (EditorUtility.DisplayDialog("Delete All Player Prefs", "You are sure to delete all player prefs?", "Yes", "No"))
            {
                Debug.Log("Delete all player prefs");
                PlayerPrefs.DeleteAll();
            }
        }
    }
}