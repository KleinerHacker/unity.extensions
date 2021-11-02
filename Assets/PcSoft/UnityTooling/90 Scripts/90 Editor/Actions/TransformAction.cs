using UnityEditor;
using UnityEngine;

namespace PcSoft.UnityTooling._90_Scripts._90_Editor.Actions
{
    public static class TransformAction
    {
        #region Setup Position

        [MenuItem("CONTEXT/Transform/Set to editor camera position", false)]
        public static void SetupToEditorPosition(MenuCommand command)
        {
            var transform = (Transform) command.context;

            transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
        }

        [MenuItem("CONTEXT/Transform/Set editor camera to position", false)]
        public static void SetupEditorToPosition(MenuCommand command)
        {
            var transform = (Transform) command.context;
            
            SceneView.lastActiveSceneView.camera.transform.position = transform.position;
            SceneView.lastActiveSceneView.camera.transform.rotation = transform.rotation;
        }

        #endregion

        #region Copy / Paste Position

        private static TransformClipboardData _transformClipboardData;

        [MenuItem("CONTEXT/Transform/Copy to clipboard", false)]
        public static void CopyTransform(MenuCommand command)
        {
            var transform = (Transform) command.context;
            _transformClipboardData = new TransformClipboardData(transform.position, transform.rotation);
        }
        
        [MenuItem("CONTEXT/Transform/Paste from clipboard", false)]
        public static void PasteTransform(MenuCommand command)
        {
            var transform = (Transform) command.context;

            transform.position = _transformClipboardData.Position;
            transform.rotation = _transformClipboardData.Rotation;
        }

        private sealed class TransformClipboardData
        {
            public Vector3 Position { get; }
            public Quaternion Rotation { get; }

            public TransformClipboardData(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }

        #endregion
    }
}