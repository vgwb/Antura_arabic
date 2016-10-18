using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    static class GenericUtilites
    {

        private static readonly System.Random _random = new System.Random(DateTime.Now.Millisecond);
        public static T GetRandom<T>(this IList<T> list)
        {
            return list[_random.Next(0, list.Count)];
        }

        public static T GetRandomEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }

        public static string ReverseText(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--) {
                reverse += cArray[i];
            }
            return reverse;
        }
    }
}
