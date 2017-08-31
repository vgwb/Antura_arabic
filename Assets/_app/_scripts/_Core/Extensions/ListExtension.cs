using System.Collections.Generic;

namespace Antura.Extensions
{
    public static class ListExtension
    {
        public static T Pull<T>(this IList<T> list)
        {
            T value = list[0];
            list.RemoveAt(0);
            return value;
        }
    }
}
