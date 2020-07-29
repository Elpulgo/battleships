using System;
using System.Collections.Generic;

namespace AI_lib
{
    internal static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new CryptoRandomizer();

            int n = list.Count;
            if (n <= 1)
                return;
                
            while (n > 1)
            {
                n--;
                int k = random.Next(0, n);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}