using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public static class ListExtension
    {
        public static T Pull< T>( this IList< T> list)
        {
            T value = list[ 0];
            list.RemoveAt( 0);
            return value;
        }

        public static T[] Shuffle< T>( this T[] arry)
        {
            int L = arry.Length;
            for( int i=0; i< 3; i++)
            for( int j=0; j< L; j++)
            {
                int r = Random.Range( 0, L);
                T temp  = arry[j];
                arry[j] = arry[r];
                arry[r] = temp;
            }

            return arry;
        }
    }
}
