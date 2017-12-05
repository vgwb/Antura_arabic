using System.Collections.Generic;
using System.Linq;

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

        public static bool ContainsAny<T>(this IEnumerable<T> thisList, IEnumerable<T> otherList)
        {
            return thisList.Any(otherList.Contains);
        }
    }
}
