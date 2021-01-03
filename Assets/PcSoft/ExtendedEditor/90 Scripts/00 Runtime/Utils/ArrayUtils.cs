using System;
using System.Collections.Generic;
using System.Linq;
using PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types;

namespace PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Utils
{
    public static class ArrayUtils
    {
        public static T[] CreateIdentifierArray<T,TE>(params TE[] excludes) where T : IIdentifiedObject<TE> where TE : Enum
        {
            var sceneStates = Enum.GetValues(typeof(TE)).Cast<TE>().ToArray();
            var list = new List<T>();

            foreach (var state in sceneStates)
            {
                if (excludes.Contains(state))
                    continue;
                
                list.Add((T) typeof(T).GetConstructor(new []{typeof(TE)}).Invoke(new object[] {state}));
            }

            return list.ToArray();
        }
        
        public static T[] UpdateIdentifierArray<T,TE>(params T[] existing) where T : IIdentifiedObject<TE> where TE : Enum
        {
            var sceneStates = Enum.GetValues(typeof(TE)).Cast<TE>().ToArray();
            var list = new List<T>();

            foreach (var state in sceneStates)
            {
                var element = existing.FirstOrDefault(x => Equals(x.Identifier, state));
                if (element != null)
                {
                    list.Add(element);
                }
                else
                {
                    list.Add((T) typeof(T).GetConstructor(new[] {typeof(TE)}).Invoke(new object[] {state}));
                }
            }

            return list.ToArray();
        }
    }
}