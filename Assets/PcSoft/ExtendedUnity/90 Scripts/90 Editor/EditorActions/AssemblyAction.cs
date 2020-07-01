using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._90_Editor.EditorActions
{
    public static class AssemblyAction
    {
        [MenuItem("Assets/Create assembly from scripts", false)]
        public static void CreateAssembly()
        {
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            var window = EditorWindow.GetWindow<AssemblyWindow>(true);
            window.ScriptRootFolder = path;
        }

        [MenuItem("Assets/Create assembly from scripts", true)]
        public static bool CanCreateAssembly()
        {
            var obj = Selection.activeObject;
            if (obj == null)
                return false;

            var path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            return Directory.Exists(path);
        }

        private static void Restructure(string scriptRoot, string assemblyName, string runtimeFolder, string runtimeName, string editorFolder, string editorName, bool deleteStructure)
        {
            //1. Rename script folder and recreate
            var scriptRootSourceDirectory = new DirectoryInfo(scriptRoot);
            var scriptRootSourceName = scriptRootSourceDirectory.Name;
            var scriptRootSourceParent = scriptRootSourceDirectory.Parent.FullName;
            scriptRootSourceDirectory.MoveTo(scriptRootSourceParent + "/" + Guid.NewGuid());
            var scriptRootTargetDirectory = Directory.CreateDirectory(scriptRootSourceParent + "/" + scriptRootSourceName);

            //2. Count directories for progress bar
            var count = scriptRootSourceDirectory.EnumerateDirectories("*.*", SearchOption.AllDirectories).Count();

            try
            {
                //3. Shift files (recursive)
                ShiftFiles(scriptRootSourceDirectory, null, scriptRootTargetDirectory, runtimeFolder, editorFolder, false, 0, count);

                //4. Create Runtime Assembly
                var runtimeAssemblyName = assemblyName + "." + runtimeName;
                File.WriteAllText(
                    scriptRootTargetDirectory.FullName + "/" + runtimeFolder + "/" + runtimeAssemblyName + ".asmdef",
                    "{ \"name\": \"" + runtimeAssemblyName + "\" }"
                );

                var editorAssemblyName = assemblyName + "." + editorName;
                File.WriteAllText(
                    scriptRootTargetDirectory.FullName + "/" + editorFolder + "/" + editorAssemblyName + ".asmdef",
                    "{ \"name\": \"" + editorAssemblyName + "\", \"includePlatforms\": [\"Editor\"], \"references\": [\"" + runtimeAssemblyName + "\"] }"
                );

                if (deleteStructure)
                {
                    scriptRootSourceDirectory.Delete(true);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

        private static void ShiftFiles(DirectoryInfo sourceRootDirectory, string sourceSubDirectory, DirectoryInfo targetRootDirectory, string runtimeFolder, string editorFolder, bool isEditor, int current, int count)
        {
            Debug.Log("Create Assembly: " + sourceSubDirectory);
            EditorUtility.DisplayProgressBar("Create Assembly", sourceSubDirectory, (float) current / (float) count);

            var sourceDirectory = GetSourceDirectory(sourceRootDirectory, sourceSubDirectory);
            var targetDirectory = GetTargetDirectory(sourceSubDirectory, targetRootDirectory, runtimeFolder, editorFolder, isEditor);

            var files = sourceDirectory.EnumerateFiles();
            var directories = sourceDirectory.EnumerateDirectories();

            foreach (var file in files)
            {
                File.Copy(file.FullName, targetDirectory.FullName + "/" + file.Name, true);
            }

            foreach (var directory in directories)
            {
                ShiftFiles(sourceRootDirectory, sourceSubDirectory + "/" + directory.Name, targetRootDirectory, runtimeFolder, editorFolder,
                    isEditor || directory.Name.EndsWith("Editor"), current + 1, count);
            }
        }

        private static DirectoryInfo GetTargetDirectory(string sourceSubDirectory, DirectoryInfo targetRootDirectory, string runtimeFolder, string editorFolder, bool isEditor)
        {
            DirectoryInfo targetDirectory;
            if (isEditor)
            {
                var match = Regex.Match(sourceSubDirectory, "\bEditor\b");
                var targetSubDirectory = sourceSubDirectory.Substring(0, match.Index) + sourceSubDirectory.Substring(match.Index + match.Length);
                targetDirectory = Directory.CreateDirectory(targetRootDirectory.FullName + "/" + editorFolder + "/" + targetSubDirectory);
            }
            else
            {
                targetDirectory = Directory.CreateDirectory(targetRootDirectory.FullName + "/" + runtimeFolder + "/" + sourceSubDirectory);
            }

            return targetDirectory;
        }

        private static DirectoryInfo GetSourceDirectory(DirectoryInfo sourceRootDirectory, string sourceSubDirectory)
        {
            return string.IsNullOrEmpty(sourceSubDirectory) ? sourceRootDirectory : new DirectoryInfo(sourceRootDirectory.FullName + "/" + sourceSubDirectory);
        }

        #region Window

        private sealed class AssemblyWindow : EditorWindow
        {
            private string _scriptRootFolder;
            private string _assemblyName;

            private string _runtimeFolderName = "Runtime";
            private string _runtimeName = "Runtime";
            private string _editorFolderName = "Editor";
            private string _editorName = "Editor";

            private bool _deleteStructure;

            #region Properties

            public string ScriptRootFolder
            {
                get => _scriptRootFolder;
                set
                {
                    _scriptRootFolder = value;
                    _assemblyName = new DirectoryInfo(value).Name;
                }
            }

            #endregion

            public AssemblyWindow()
            {
                titleContent = new GUIContent("Create Assembly");
            }

            private void OnEnable()
            {
                minSize = new Vector2(250f, 400f);
                maxSize = minSize;
            }

            private void OnGUI()
            {
                EditorGUILayout.LabelField("Script Root Folder:");
                _scriptRootFolder = EditorGUILayout.TextField(_scriptRootFolder);

                EditorGUILayout.LabelField("Assembly Name");
                _assemblyName = EditorGUILayout.TextField(_assemblyName);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Runtime Assembly Postfix:");
                _runtimeName = EditorGUILayout.TextField(_runtimeName);

                EditorGUILayout.LabelField("Editor Assembly Postfix:");
                _editorName = EditorGUILayout.TextField(_editorName);

                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Target Runtime Folder Name:");
                _runtimeFolderName = EditorGUILayout.TextField(_runtimeFolderName);

                EditorGUILayout.LabelField("Target Editor Folder Name:");
                _editorFolderName = EditorGUILayout.TextField(_editorFolderName);

                _deleteStructure = EditorGUILayout.Toggle("Delete original structure", _deleteStructure);

                EditorGUILayout.Space(100);

                if (GUILayout.Button("Create"))
                {
                    Close();
                    Restructure(_scriptRootFolder, _assemblyName, _runtimeFolderName, _runtimeName, 
                        _editorFolderName, _editorName, _deleteStructure);
                }
            }
        }

        #endregion
    }
}