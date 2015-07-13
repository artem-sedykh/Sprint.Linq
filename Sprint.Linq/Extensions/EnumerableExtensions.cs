using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprint.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            return DistinctByIterator(source, keySelector);
        }

        private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>
            (IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (keySelector == null)
                throw new ArgumentNullException("keySelector");

            var knownKeys = new HashSet<TKey>();

            return source.Where(element => knownKeys.Add(keySelector(element))).ToList();
        }
    }
}
