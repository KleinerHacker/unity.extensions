using System.IO;
using System.Linq;
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

        private static void Restructure(string scriptRoot, string assemblyName, string runtimeFolder, string editorFolder, bool move)
        {
            var count = Directory.EnumerateDirectories(scriptRoot, "*.*", SearchOption.AllDirectories).Count();

            try
            {
                ShiftFiles(scriptRoot, null, runtimeFolder, editorFolder, false, move, 0, count);

                var runtimeAssemblyName = assemblyName + "." + runtimeFolder;
                File.WriteAllText(
                    scriptRoot + "/" + runtimeFolder + "/" + runtimeAssemblyName + ".asmdef",
                    "{ \"name\": \"" + runtimeAssemblyName + "\" }"
                );

                var editorAssemblyName = assemblyName + "." + editorFolder;
                File.WriteAllText(
                    scriptRoot + "/" + editorFolder + "/" + editorAssemblyName + ".asmdef",
                    "{ \"name\": \"" + editorAssemblyName + "\", \"includePlatforms\": [\"Editor\"], \"references\": [\"" + runtimeAssemblyName + "\"] }"
                );
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
        }

        private static void ShiftFiles(string scriptRoot, string subDirectory, string runtimeFolder, string editorFolder, bool isEditor, bool move, int current, int count)
        {
            EditorUtility.DisplayProgressBar("Create Assembly", subDirectory, (float) current / (float) count);

            var currentDir = string.IsNullOrEmpty(subDirectory) ? scriptRoot : scriptRoot + "/" + subDirectory;

            var files = Directory.EnumerateFiles(currentDir);
            var directories = Directory.EnumerateDirectories(currentDir);

            DirectoryInfo dir;
            if (isEditor)
            {
                var newSubDirectory = subDirectory.Replace("Editor", "");
                dir = Directory.CreateDirectory(scriptRoot + "/" + editorFolder + "/" + newSubDirectory);
            }
            else
            {
                dir = Directory.CreateDirectory(scriptRoot + "/" + runtimeFolder + "/" + subDirectory);
            }

            foreach (var fileName in files)
            {
                var file = new FileInfo(fileName);
                if (move)
                {
                    File.Move(file.FullName, dir.FullName + "/" + file.Name);
                }
                else
                {
                    File.Copy(file.FullName, dir.FullName + "/" + file.Name);
                }
            }

            foreach (var directory in directories)
            {
                ShiftFiles(scriptRoot, subDirectory + "/" + new DirectoryInfo(directory).Name, runtimeFolder, editorFolder,
                    isEditor || directory.EndsWith("Editor"), move, current + 1, count);
            }
            
            if (move && !string.IsNullOrEmpty(subDirectory))
            {
                Directory.Delete(scriptRoot + "/" + subDirectory, true);
            }
        }

        #region Window

        private sealed class AssemblyWindow : EditorWindow
        {
            private string _scriptRootFolder;
            private string _assemblyName;

            private string _runtimeFolderName = "Runtime";
            private string _editorFolderName = "Editor";

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

                EditorGUILayout.LabelField("Target Runtime Folder Name:");
                _runtimeFolderName = EditorGUILayout.TextField(_runtimeFolderName);

                EditorGUILayout.LabelField("Target Editor Folder Name:");
                _editorFolderName = EditorGUILayout.TextField(_editorFolderName);

                EditorGUILayout.Space();

                _deleteStructure = EditorGUILayout.Toggle("Delete original structure", _deleteStructure);

                EditorGUILayout.Space(100);

                if (GUILayout.Button("Create"))
                {
                    Close();
                    Restructure(_scriptRootFolder, _assemblyName, _runtimeFolderName, _editorFolderName, _deleteStructure);
                }
            }
        }

        #endregion
    }
}