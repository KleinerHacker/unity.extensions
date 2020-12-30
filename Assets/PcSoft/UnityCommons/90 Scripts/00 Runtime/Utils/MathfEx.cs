using UnityEngine;

namespace PcSoft.UnityCommons._90_Scripts._00_Runtime.Utils
{
    public static class MathfEx
    {
        public const float ToRadiant = 2f * Mathf.PI / 360f;
    }

    public struct Box
    {
        #region Static Area

        public static Box FromPoints(Vector3 point1, Vector3 point2)
        {
            var size = new Vector3(
                CalculateFullDif(point1.x, point2.x),
                CalculateFullDif(point1.y, point2.y),
                CalculateFullDif(point1.z, point2.z)
            );
            var relativeCenter = new Vector3(
                CalculateHalfDif(point1.x, point2.x),
                CalculateHalfDif(point1.y, point2.y),
                CalculateHalfDif(point1.z, point2.z)
            );

            var origin = new Vector3(
                Mathf.Min(point1.x, point2.x),
                Mathf.Min(point1.y, point2.y),
                Mathf.Min(point1.z, point2.z)
            );
            var center = origin + relativeCenter;

            return new Box(center, size);
        }

        private static float CalculateHalfDif(float p1, float p2)
        {
            return CalculateFullDif(p1, p2) / 2f;
        }

        private static float CalculateFullDif(float p1, float p2)
        {
            return p1 > p2 ? p1 - p2 : p2 - p1;
        }

        #endregion

        public Vector3 Center { get; }
        public Vector3 Size { get; }

        public Box(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
        }

        public bool IsInBox(Vector3 point, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false)
        {
            var halfSize = Size / 2f;
            return (ignoreX || IsInRange(Center.x - halfSize.x, Center.x + halfSize.x, point.x)) &&
                   (ignoreY || IsInRange(Center.y - halfSize.y, Center.y + halfSize.y, point.y)) &&
                   (ignoreZ || IsInRange(Center.z - halfSize.z, Center.z + halfSize.z, point.z));
        }

        private bool IsInRange(float min, float max, float value)
        {
            return value >= min && value <= max;
        }

        #region Equals / Hashcode / ToString

        public bool Equals(Box other)
        {
            return Center.Equals(other.Center) && Size.Equals(other.Size);
        }

        public override bool Equals(object obj)
        {
            return obj is Box other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Center.GetHashCode() * 397) ^ Size.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{nameof(Center)}: {Center}, {nameof(Size)}: {Size}";
        }

        #endregion
    }
}