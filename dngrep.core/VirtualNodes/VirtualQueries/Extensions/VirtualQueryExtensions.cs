using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using Microsoft.CodeAnalysis;

namespace dngrep.core.VirtualNodes.VirtualQueries.Extensions
{
    public static class VirtualQueryExtensions
    {
        public static CombinedSyntaxNode QueryVirtualAndCombine(
            this SyntaxNode node,
            params IVirtualNodeQuery[] queries)
        {
            _ = node ?? throw new ArgumentNullException(nameof(node));
            _ = queries ?? throw new ArgumentNullException(nameof(queries));

            if (queries.Length < 1)
            {
                throw new ArgumentException(
                    "You should provide at least one query",
                    nameof(queries));
            }

            if (queries.Any(x => !x.HasOverride))
            {
                throw new ArgumentException(
                    "Only queries that override the target are supported.");
            }

            IEnumerable<IVirtualNodeQuery> supported = queries.Where(q => q.CanQuery(node));

            IVirtualNodeQuery? query = null;

            if (supported.Count() > 1)
            {
                query = supported.GetSingleOverride();
            }
            else if (supported.Count() == 1)
            {
                query = supported.Single();
            }

            return query == null
                ? new CombinedSyntaxNode(node)
                : new CombinedSyntaxNode(query.Query(node));
        }

        public static CombinedSyntaxNode[] QueryVirtualAndCombine(
            this IEnumerable<SyntaxNode> nodes,
            params IVirtualNodeQuery[] queries)
        {
            _ = nodes ?? throw new ArgumentNullException(nameof(nodes));
            _ = queries ?? throw new ArgumentNullException(nameof(queries));

            if (queries.Length < 1)
            {
                throw new ArgumentException(
                    "You should provide at least one query",
                    nameof(queries));
            }

            if (queries.Any(x => !x.HasOverride))
            {
                throw new ArgumentException(
                    "Only queries that override the target are supported.");
            }

            return nodes.Select(x =>
            {
                IEnumerable<IVirtualNodeQuery> supported = queries.Where(q => q.CanQuery(x));

                IVirtualNodeQuery? query = null;

                if (supported.Count() > 1)
                {
                    query = supported.GetSingleOverride();
                }
                else if (supported.Count() == 1)
                {
                    query = supported.Single();
                }

                return query == null
                    ? new CombinedSyntaxNode(x)
                    : new CombinedSyntaxNode(query.Query(x));
            }).ToArray();
        }
    }
}
