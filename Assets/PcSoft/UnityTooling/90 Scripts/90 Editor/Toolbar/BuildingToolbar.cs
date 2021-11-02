using System;
using System.Linq;
using PcSoft.UnityTooling._90_Scripts._90_Editor.Provider;
using PcSoft.UnityTooling._90_Scripts._90_Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace PcSoft.UnityTooling._90_Scripts._90_Editor.Toolbar
{
    [InitializeOnLoad]
    public static class BuildingToolbar
    {
        private static BuildTarget _buildTarget;
        private static int _buildType;
        private static BuildExtras _buildExtras;
        private static bool _clean;

        static BuildingToolbar()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
            _buildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            GUILayout.Space(15f);

            GUILayout.Label("Target: ", ToolbarStyles.labelStyle);
            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(null, _buildTarget, v => UnityHelper.IsBuildTargetSupported((BuildTarget)v), false,   
                ToolbarStyles.popupStyle, ToolbarLayouts.popupLayout);
            if (GUILayout.Button("Reset", ToolbarStyles.commandButtonStyle))
            {
                _buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }

            GUILayout.Space(5f);

            GUILayout.Label("Type: ", ToolbarStyles.labelStyle);
            _buildType = EditorGUILayout.Popup(_buildType, BuildingSettings.Singleton.TypeItems.Select(x => x.Name).ToArray(),
                ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);

            GUILayout.Space(5f);
            
            GUILayout.Label("Extras: ", ToolbarStyles.labelStyle);
            _buildExtras = (BuildExtras)EditorGUILayout.EnumFlagsField(_buildExtras, ToolbarStyles.popupStyle, ToolbarLayouts.popupSmallLayout);
            
            GUILayout.Space(5f);

            _clean = GUILayout.Toggle(_clean, new GUIContent("Clean", "Clean complete build cache"), ToolbarStyles.toggleStyle);
            
            GUILayout.Space(5f);

            if (GUILayout.Button(new GUIContent("Build", "Build Project"), ToolbarStyles.commandButtonStyle))
            {
                Build(false);
            }

            if (GUILayout.Button(new GUIContent("Run", "Build and Run project"), ToolbarStyles.commandButtonStyle))
            {
                Build(true);
            }
        }

        private static void Build(bool run)
        {
            UnityBuilding.Build(_buildTarget, _buildType, _buildExtras, run, _clean);
        }

        private static class ToolbarLayouts
        {
            public static readonly GUILayoutOption[] popupLayout;
            public static readonly GUILayoutOption[] popupSmallLayout;

            static ToolbarLayouts()
            {
                popupLayout = new[]
                {
                    GUILayout.Width(150f)
                };
                popupSmallLayout = new[]
                {
                    GUILayout.Width(100f)
                };
            }
        }

        private static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;
            public static readonly GUIStyle popupStyle;
            public static readonly GUIStyle labelStyle;
            public static readonly GUIStyle toggleStyle;

            static ToolbarStyles()
            {
                commandButtonStyle = new GUIStyle("Command")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal,
                    fixedWidth = 50f
                };

                popupStyle = new GUIStyle("Popup")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    stretchWidth = false
                };

                labelStyle = new GUIStyle("Label")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    fixedHeight = 20f
                };
                
                toggleStyle = new GUIStyle("Toggle")
                {
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    imagePosition = ImagePosition.TextOnly,
                    fontStyle = FontStyle.Normal,
                    fixedHeight = 20f
                };
            }
        }
        
        [Flags]
        public enum BuildExtras
        {
            None = 0x00,
            CodeCoverage = 0x01,
            UseProfiler = 0x02,
            StrictMode = 0x04,
        }
    }
}