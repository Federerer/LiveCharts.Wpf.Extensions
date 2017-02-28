using System;
using System.Collections.Generic;

namespace LiveCharts.Wpf.Extensions.Utils
{
    internal static class IListExtensions
    {
        public static int RemoveAll<T>(this IList<T> list, Predicate<T> match)
        {
            var count = 0;

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                {
                    ++count;
                    list.RemoveAt(i);
                }
            }

            return count;
        }
    }
}