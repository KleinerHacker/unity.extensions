using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace PcSoft.AudioMachine._90_Scripts.Utils.Extensions
{
    internal static class ListRandomExtension
    {
        public static T GetRandomItem<T>(this IEnumerable<T> list)
        {
            if (list.Count() <= 0)
                return default;
            if (list.Count() == 1)
                return list.ElementAt(0);

            return list.ElementAt(Random.Range(0, list.Count()));
        }

        public static T GetRandomItem<T>(this IEnumerable<T> list, T currentItem)
        {
            if (currentItem == null)
                return GetRandomItem(list);
            
            if (list.Count() <= 0)
                return default;
            if (list.Count() == 1)
                return list.ElementAt(0);

            T item;
            do
            {
                item = list.ElementAt(Random.Range(0, list.Count()));
            } while (Object.Equals(item, currentItem));

            return item;
        }
    }
}