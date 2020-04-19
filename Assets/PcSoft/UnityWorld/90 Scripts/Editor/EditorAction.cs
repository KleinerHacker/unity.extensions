using System.Linq;
using PcSoft.UnityWorld._90_Scripts.Assets;
using UnityEditor;

namespace PcSoft.UnityWorld._90_Scripts.Editor
{
    public static class EditorAction
    {
        #region Open World Group

        [MenuItem("Assets/Open World Group/One", false, 0)]
        public static void OpenWorldGroupOne()
        {
            LoadWorldGroup(WorldSceneGroup.One);
        }

        [MenuItem("Assets/Open World Group/One", true, 0)]
        public static bool CanOpenWorldGroupOne()
        {
            return Selection.activeObject is WorldAsset;
        }

        [MenuItem("Assets/Open World Group/Two", false, 0)]
        public static void OpenWorldGroupTwo()
        {
            LoadWorldGroup(WorldSceneGroup.Two);
        }

        [MenuItem("Assets/Open World Group/Two", true, 0)]
        public static bool CanOpenWorldGroupTwo()
        {
            return Selection.activeObject is WorldAsset;
        }
        
        [MenuItem("Assets/Open World Group/Three", false, 0)]
        public static void OpenWorldGroupThree()
        {
            LoadWorldGroup(WorldSceneGroup.Three);
        }

        [MenuItem("Assets/Open World Group/Three", true, 0)]
        public static bool CanOpenWorldGroupThree()
        {
            return Selection.activeObject is WorldAsset;
        }

        private static void LoadWorldGroup(WorldSceneGroup group)
        {
            var world = Selection.activeObject as WorldAsset;
            var scenes = world.Scenes.Where(x => x.Group == group).ToArray();
            if (scenes.Length <= 0)
            {
                EditorUtility.DisplayDialog("Open World Group", "No scenes are associated to this world group", "OK");
                return;
            }

            EditorActionUtils.LoadScenes(scenes);
        }

        #endregion
    }
}