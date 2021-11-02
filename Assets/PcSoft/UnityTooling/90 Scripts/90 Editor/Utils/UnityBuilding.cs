using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.UnityTooling._90_Scripts._90_Editor.Provider;
using PcSoft.UnityTooling._90_Scripts._90_Editor.Toolbar;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace PcSoft.UnityTooling._90_Scripts._90_Editor.Utils
{
    public static class UnityBuilding
    {
        private const string TargetKey = "${TARGET}";
        internal const string DefaultTargetPath = "Builds/" + TargetKey;

        public static void Build(BuildTarget buildTarget, int buildTypeIndex, BuildingToolbar.BuildExtras buildExtras, bool run, bool clean)
        {
            var buildingSettings = BuildingSettings.Singleton;
            var buildingType = buildingSettings.TypeItems[buildTypeIndex];

            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var cppCompilerConfiguration = CalculateConfiguration(buildingType);
            if (cppCompilerConfiguration.HasValue)
            {
                PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup, cppCompilerConfiguration.Value);
            }
            PlayerSettings.SetManagedStrippingLevel(buildTargetGroup, buildingType.StrippingLevel);

            var options = new BuildPlayerOptions
            {
                scenes = KnownScenes,
                target = buildTarget,
                locationPathName = DefaultTargetPath.Replace(TargetKey, buildTarget.ToString()) + buildingType.TargetPath,
                options = CalculateOptions(buildingType, buildExtras, run, clean),
                extraScriptingDefines = EditorUserBuildSettings.activeScriptCompilationDefines.Concat(buildingType.Defines).ToArray()
            };

            var buildReport = BuildPipeline.BuildPlayer(options);
        }

        private static string[] KnownScenes
        {
            get
            {
                var scenes = new List<string>();
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    scenes.Add(scene.path);
                }

                return scenes.ToArray();
            }
        }

        private static BuildOptions CalculateOptions(BuildingTypeItem buildingType, BuildingToolbar.BuildExtras buildExtras, bool autoRun, bool clean)
        {
            var options = BuildOptions.None;
            if (buildingType.Compress)
            {
                options |= BuildOptions.CompressWithLz4HC;
            }

            if (buildingType.AllowDebugging)
            {
                options |= BuildOptions.AllowDebugging;
            }

            if (buildingType.DevelopmentBuild)
            {
                options |= BuildOptions.Development;
            }

            if (buildExtras.HasFlag(BuildingToolbar.BuildExtras.CodeCoverage))
            {
                options |= BuildOptions.EnableCodeCoverage;
            }
            
            if (buildExtras.HasFlag(BuildingToolbar.BuildExtras.StrictMode))
            {
                options |= BuildOptions.StrictMode;
            }
            
            if (buildExtras.HasFlag(BuildingToolbar.BuildExtras.UseProfiler))
            {
                options |= BuildOptions.ConnectWithProfiler;
                options |= BuildOptions.EnableDeepProfilingSupport;
            }

            if (autoRun)
            {
                options |= BuildOptions.AutoRunPlayer;
            }

            if (clean)
            {
                options |= BuildOptions.CleanBuildCache;
            }

            return options;
        }

        private static Il2CppCompilerConfiguration? CalculateConfiguration(BuildingTypeItem item)
        {
            return item.CPPSettings switch
            {
                IL2CPPSettings.Deactivated => null,
                IL2CPPSettings.Debug => Il2CppCompilerConfiguration.Debug,
                IL2CPPSettings.Master => Il2CppCompilerConfiguration.Master,
                IL2CPPSettings.Release => Il2CppCompilerConfiguration.Release,
                _ => throw new NotImplementedException()
            };
        }
    }
}