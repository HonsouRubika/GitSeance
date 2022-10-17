/// Author: Nicolas Capelier
/// Last modified by: Nicolas Capelier

using System.Collections.Generic;
using UnityEngine;

namespace Seance.Utility
{
    public static class ListExtension
    {
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
