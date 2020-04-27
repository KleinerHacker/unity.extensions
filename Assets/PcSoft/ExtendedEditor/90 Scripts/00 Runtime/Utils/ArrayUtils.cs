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
    }
}