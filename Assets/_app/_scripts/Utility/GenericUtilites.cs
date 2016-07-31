using UnityEngine;
using System.Collections;

namespace EA4S
{
    static class GenericUtilites
    {
        public static T GetRandomEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
            return V;
        }
    }
}
