using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PcSoft.UnityCommons._90_Scripts._00_Runtime.Utils.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<(T, float)> ToWeightList<T>(this IEnumerable<T> list, Func<T, float> weightExtractor)
        {
            return list.Select(x => (x, weightExtractor(x)));
        }

        public static T GetRandom<T>(this IEnumerable<T> list, params T[] excludes)
        {
            var filteredList = list.Where(x => !excludes.Contains(x)).ToList();
            if (filteredList.Count <= 0)
                return default;
            
            return filteredList.ElementAt(Random.Range(0, filteredList.Count()));
        }

        public static T GetRandom<T>(this IEnumerable<T> list, Predicate<T> excludePredicate)
        {
            return GetRandom(list, list.Where(x => excludePredicate(x)).ToArray());
        }

        public static T GetRandomByWeight<T>(this IEnumerable<(T, float)> list, params T[] excludes)
        {
            var filteredList = list.Where(x => !excludes.Contains(x.Item1)).ToList();
            if (filteredList.Count <= 0)
                return default;
            
            var weightSum = filteredList.Sum(x => x.Item2);

            var randomWeight = Random.Range(0, weightSum);
            var weightCounter = 0f;
            foreach (var item in filteredList)
            {
                weightCounter += item.Item2;
                if (weightCounter >= randomWeight)
                    return item.Item1;
            }

            return filteredList.Last().Item1;
        }

        public static IEnumerable<T> GetRandomList<T>(this IEnumerable<T> list, float percentage, params T[] excludes)
        {
            var filteredList = list.Where(x => !excludes.Contains(x)).ToList();
            if (filteredList.Count <= 0)
                return filteredList;
            
            var tmpList = new List<T>();
            var count = (int) (filteredList.Count * Mathf.Clamp01(percentage));

            if (count == filteredList.Count)
                return filteredList;

            for (var i = 0; i < count; i++)
            {
                T randomItem;
                do
                {
                    randomItem = filteredList.GetRandom();
                } while (tmpList.Contains(randomItem));

                tmpList.Add(randomItem);
            }

            return tmpList;
        }

        public static IEnumerable<T> Remove<T>(this IEnumerable<T> list, T item)
        {
            return list.Where(x => !Equals(x, item));
        }
    }
}