using System;
using System.Linq;
using Assets.PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Types;

namespace Assets.PcSoft.ExtendedEditor._90_Scripts._00_Runtime.Utils
{
    public static class ArrayUtils
    {
        public static T[] CreateIdentifierArray<T,TE>() where T : IIdentifiedObject<TE> where TE : Enum
        {
            var sceneStates = Enum.GetValues(typeof(TE)).Cast<TE>().ToArray();
            var array = new T[sceneStates.Length];

            for (var i = 0; i < sceneStates.Length; i++)
            {
                array[i] = (T) typeof(T).GetConstructor(new []{typeof(TE)}).Invoke(new object[] {sceneStates[i]});
            }

            return array;
        }
    }
}