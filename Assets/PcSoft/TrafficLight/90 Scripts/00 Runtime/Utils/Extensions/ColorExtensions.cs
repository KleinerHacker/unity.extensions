using UnityEngine;

namespace PcSoft.TrafficLight._90_Scripts._00_Runtime.Utils.Extensions
{
    internal static class ColorExtensions
    {
        public static float GetIntensity(this Color color)
        {
            return (color.r + color.g + color.b) / 3f;
        }

        public static Color Normalize(this Color color)
        {
            var intensity = GetIntensity(color);
            var factor = 1f / intensity;
            
            return new Color(color.r * factor, color.g * factor, color.b * factor);
        }
    }
}