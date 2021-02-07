using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Queries;

namespace dngrep.core.VirtualNodes.Routings.ConflictResolution
{
    public interface IVirtualQueryOverrideRouting
    {
        IVirtualNodeQuery GetSingleOverride(IEnumerable<IVirtualNodeQuery> queries);
    }

    public static class VirtualQueryOverrideRoutingExtensions
    {
        private static readonly IVirtualQueryOverrideRouting DefaultRouting
            = new VirtualQueryOverrideRouting();

        public static IVirtualNodeQuery GetSingleOverride(
            this IEnumerable<IVirtualNodeQuery> queries)
        {
            return DefaultRouting.GetSingleOverride(queries);
        }
    }

    public class VirtualQueryOverrideRouting : IVirtualQueryOverrideRouting
    {
        public IVirtualNodeQuery GetSingleOverride(IEnumerable<IVirtualNodeQuery> queries)
        {
            _ = queries ?? throw new ArgumentNullException(nameof(queries));

            if (queries.Count() < 2)
            {
                throw new ArgumentException(
                    "Overrides should only be applied for two or more queries.",
                    nameof(queries));
            }

            Type[] overrides = queries
                .SelectMany(
                    x => x.GetType()
                        .GetInterfaces()
                        .Where(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(ICanOverride<>)))
                .Select(x => x.GetGenericArguments().First())
                .ToArray();

            Type[] distinctOverrides = overrides.Distinct().ToArray();

            if (overrides.Length != distinctOverrides.Length)
            {
                IEnumerable<Type> duplicateTypes = overrides
                    .GroupBy(x => x)
                    .Where(x => x.Count() > 1)
                    .Select(x => x.First());

                if (queries.Any(x => duplicateTypes.Any(d => d.IsInstanceOfType(x))))
                {
                    throw new InvalidOperationException("Conflicting override detected.");
                }
            }

            return queries.Single(x => !overrides.Any(o => o.IsInstanceOfType(x)));
        }
    }
}
