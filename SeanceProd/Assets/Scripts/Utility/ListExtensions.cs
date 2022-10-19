/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections.Generic;
using UnityEngine;

namespace Seance.Utility
{
    public static class ListExtensions
    {
        public static T PickRandom<T>(this List<T> list)
        {
            if(list == null || list.Count == 0)
                throw new System.IndexOutOfRangeException();

            return list[Random.Range(0, list.Count - 1)];
        }

        public static List<T> FisherYates<T>(this List<T> list)
        {
            int random;

            for (int i = 0; i < list.Count; i++)
            {
                random = Random.Range(0, list.Count - 1);
                (list[i], list[random]) = (list[random], list[i]);
            }

            return list;
        }
    }
}
