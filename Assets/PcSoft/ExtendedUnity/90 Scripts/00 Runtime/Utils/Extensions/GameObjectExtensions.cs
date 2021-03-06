using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PcSoft.ExtendedUnity._90_Scripts._00_Runtime.Utils.Extensions
{
    public static class GameObjectExtensions
    {
        public static IEnumerable<GameObject> InstantiateAll(this IEnumerable<GameObject> list)
        {
            return list.Select(GameObject.Instantiate).ToArray();
        }
        
        public static IEnumerable<GameObject> InstantiateAll(this IEnumerable<GameObject> list, Vector3 pos, Quaternion rot, Transform transform = null)
        {
            return list.Select(x => GameObject.Instantiate(x, pos, rot, transform)).ToArray();
        }

        public static void DestroyAll(this IEnumerable<GameObject> list) 
        {
            foreach (var o in list)
            {
                GameObject.Destroy(o);
            }
        }
    }
}