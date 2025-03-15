using System;

namespace dngrep.core.Extensions.Nullable
{
    public static class NullableExtensions
    {
        public static T NotNull<T>(this T? node)
            where T : class
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));

            return node;
        }
    }
}
