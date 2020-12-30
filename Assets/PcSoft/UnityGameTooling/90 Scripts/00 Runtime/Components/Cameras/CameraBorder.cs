using PcSoft.UnityCommons._90_Scripts._00_Runtime.Utils;
using UnityEngine;

namespace PcSoft.UnityGameTooling._90_Scripts._00_Runtime.Components.Cameras
{
    [AddComponentMenu(UnityGameToolingConstants.Menu.Component.CameraMenu + "/Camera Border")]
    [DisallowMultipleComponent]
    public sealed class CameraBorder : MonoBehaviour
    {
        #region Inspector Data

        [SerializeField]
        private Transform borderPoint1;

        [SerializeField]
        private Transform borderPoint2;

        #endregion

        private Box _box;

        #region Builtin Methods

        private void Awake()
        {
            _box = Box.FromPoints(borderPoint1.position, borderPoint2.position);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (borderPoint1 == null || borderPoint2 == null)
                return;

            var box = Box.FromPoints(borderPoint1.position, borderPoint2.position);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(box.Center, box.Size);
        }

        private void OnDrawGizmosSelected()
        {
            if (borderPoint1 == null || borderPoint2 == null)
                return;

            var box = Box.FromPoints(borderPoint1.position, borderPoint2.position);

            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawCube(box.Center, box.Size);
        }
#endif

        #endregion

        public bool InBox(Vector3 pos)
        {
            return _box.IsInBox(pos, ignoreY: true);
        }
    }
}