using System.Collections.Generic;
using UnityEngine;

namespace Solitaire.Tools
{
    public static class RandomExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var n = Random.Range(0, i + 1);
                (list[i], list[n]) = (list[n], list[i]);
            }
        } 
    }
}