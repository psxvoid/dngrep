using System.Collections.Generic;
using System.Linq;

namespace dngrep.core.Extensions.EnumerableExtensions
{
    public static class EmptyItemsExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? items)
        {
            return items == null || !items.Any();
        }

        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T>? items)
        {
            return items == null || items.Count == 0;
        }
    }
}
