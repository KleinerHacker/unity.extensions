using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PcSoft.ExtendedEditor._90_Scripts._90_Editor
{
    public static class ExtendedEditorGUI
    {
        public static T AssetPopup<T>(Rect rect, Func<T, string> nameExtractor, SerializedProperty property, Action<T> onChanged = null) where T : Object
        {
            var assetNames = AssetDatabase.FindAssets("t:" + typeof(T).Name)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => (T)AssetDatabase.LoadAssetAtPath(path, typeof(T)))
                .Select(nameExtractor)
                .ToArray();

            var assetIndex = property.objectReferenceValue == null ? -1 : assetNames.ToList().IndexOf(nameExtractor((T) property.objectReferenceValue));
            var lastIndex = assetIndex;

            assetIndex = EditorGUI.Popup(rect, assetIndex, assetNames);
            var newAsset = assetIndex < 0 || assetIndex >= assetNames.Length ? null : AssetDatabase.FindAssets("t:" + typeof(T).Name)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => (T) AssetDatabase.LoadAssetAtPath(path, typeof(T)))
                .FirstOrDefault(asset => nameExtractor(asset) == assetNames[assetIndex]);
            
            if (assetIndex != lastIndex)
            {
                Debug.Log("Setup Asset from Popup: " + typeof(T).Name + " / " + newAsset);
                property.objectReferenceValue = newAsset;
                
                onChanged?.Invoke(newAsset);
            }

            return newAsset;
        }
    }
}