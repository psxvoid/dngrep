using System;
using Lamar;

namespace dngrep.tool.Core.Output.Presenters
{
    public enum PresenterKind
    {
        Search,
        Statistics
    }

    public interface IPresenterFactory
    {
        ISyntaxNodePresenter GetPresenter(PresenterKind kind);
    }

    public class PresenterFactory : IPresenterFactory
    {
        private readonly IContainer container;

        public PresenterFactory(IContainer container)
        {
            this.container = container;
        }

        public ISyntaxNodePresenter GetPresenter(PresenterKind kind)
        {
            Type target = kind switch
            {
                PresenterKind.Search => typeof(ConsoleSyntaxNodePresenter),
                PresenterKind.Statistics => typeof(SyntaxNodeStatisticsConsolePresenter),
                _ => throw new InvalidOperationException("Unknown presenter kind.")
            };

            return this.container.GetInstance(target) as ISyntaxNodePresenter
                ?? throw new InvalidOperationException(
                    "Unable to get an instance from the container");
        }
    }
}
