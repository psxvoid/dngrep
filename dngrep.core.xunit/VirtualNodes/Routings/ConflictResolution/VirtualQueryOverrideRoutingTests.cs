using System;
using dngrep.core.Queries;
using dngrep.core.VirtualNodes;
using dngrep.core.VirtualNodes.Routings.ConflictResolution;
using Microsoft.CodeAnalysis;
using Xunit;

namespace dngrep.core.xunit.VirtualNodes.Routings.ConflictResolution
{
    public class VirtualQueryOverrideRoutingTests
    {
        #region MockQueries

        private abstract class BaseQuery : IVirtualNodeQuery
        {
            public bool HasOverride => throw new System.NotImplementedException();

            public bool CanQuery(SyntaxNode node)
            {
                throw new System.NotImplementedException();
            }

            public IVirtualSyntaxNode Query(SyntaxNode node)
            {
                throw new System.NotImplementedException();
            }
        }

        private class HighestPriorityQuery
            : BaseQuery,
            ICanOverride<MidPriorityQuery>,
            ICanOverride<LowestPriorityQuery>
        {
        }

        private class MidPriorityQuery : BaseQuery, ICanOverride<LowestPriorityQuery>
        {
        }

        private class LowestPriorityQuery : BaseQuery
        {
        }

        #endregion

        [Fact]
        public void GetSingleOverride_SingleQuery_Throws()
        {
            var queries = new IVirtualNodeQuery[] {
                new LowestPriorityQuery(),
            };

            Assert.Throws<ArgumentException>(() => queries.GetSingleOverride());
        }

        [Fact]
        public void GetSingleOverride_LowestAndMidSingleOverride_MidPriority()
        {
            var queries = new IVirtualNodeQuery[] {
                new LowestPriorityQuery(),
                new MidPriorityQuery()
            };

            Assert.IsType<MidPriorityQuery>(queries.GetSingleOverride());
        }

        [Fact]
        public void GetSingleOverride_LowestAndHighestSingleOverride_HighestPriority()
        {
            var queries = new IVirtualNodeQuery[] {
                new LowestPriorityQuery(),
                new HighestPriorityQuery()
            };

            Assert.IsType<HighestPriorityQuery>(queries.GetSingleOverride());
        }

        [Fact]
        public void GetSingleOverride_MidAndHighestSingleOverride_HighestPriority()
        {
            var queries = new IVirtualNodeQuery[] {
                new MidPriorityQuery(),
                new HighestPriorityQuery()
            };

            Assert.IsType<HighestPriorityQuery>(queries.GetSingleOverride());
        }

        [Fact]
        public void GetSingleOverride_TwoOverrideSameQuery_Throws()
        {
            var queries = new IVirtualNodeQuery[] {
                new LowestPriorityQuery(),
                new MidPriorityQuery(),     // can override the lowest priority node
                new HighestPriorityQuery(), // also can override the lowest priority node
            };

            Assert.Throws<InvalidOperationException>(() => queries.GetSingleOverride());
        }
    }
}
