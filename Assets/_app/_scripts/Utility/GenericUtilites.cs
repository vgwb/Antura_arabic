using System;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    internal static class GenericUtilites
    {
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);
        public static T GetRandom<T>(this IList<T> list)
        {
            return list[_random.Next(0, list.Count)];
        }

        public static T GetRandomEnum<T>()
        {
            var A = Enum.GetValues(typeof(T));
            var V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }

        public static string ReverseText(string text)
        {
            var cArray = text.ToCharArray();
            var reverse = string.Empty;
            for (var i = cArray.Length - 1; i > -1; i--) {
                reverse += cArray[i];
            }
            return reverse;
        }

        public static double GetTimestamp()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return timeSpan.TotalSeconds;
        }

    }
}
