using System.Collections.Generic;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Utils.Extensions
{
    internal static class DictionaryExtensions
    {
        public static void AddRange<TK, TV>(this IDictionary<TK, TV> dict, IDictionary<TK, TV> other)
        {
            foreach (var kv in other)
            {
                dict.Add(kv.Key, kv.Value);
            }
        }
    }
}