using System.Collections;
using System.Linq;
using UnityEngine;

namespace PcSoft.SaveGame._90_Scripts.Utils.Extensions
{
    internal static class EnumerationExtensions
    {
        public static int Count(this IEnumerable e)
        {
            return Enumerable.Count(e.Cast<object>());
        }

        public static object ElementAt(this IEnumerable e, int index)
        {
            return Enumerable.ElementAt(e.Cast<object>(), index);
        }
    }
}