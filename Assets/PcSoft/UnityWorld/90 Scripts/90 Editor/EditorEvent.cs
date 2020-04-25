using Assets.PcSoft.UnityWorld._90_Scripts._00_Runtime.Assets;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Assets.PcSoft.UnityWorld._90_Scripts._90_Editor
{
    public static class EditorEvent
    {
        [OnOpenAsset]
        public static bool OnOpenWorld(int instanceID, int line)
        {
            var world = EditorUtility.InstanceIDToObject(instanceID) as WorldAsset;
            if (world == null || world.Scenes.Length <= 0)
                return false;

            EditorActionUtils.LoadScenes(world.Scenes);
            return true;
        }
    }
}